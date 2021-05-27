using System.Collections;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MyBox;

public class LobbyNetworkManager : MonoBehaviourPunCallbacks
{
    #region Private fields

    [Tooltip("Name of the scene to load after player joins a room")]
    [SerializeField]
    private string sceneNameToLoadIn = "Starting Level";

    private string gameVersion = "1";

    // Player affordances to customizing a room
    [Separator("Room customization options")]
    [SerializeField] private InputField m_roomNameInput;
    [SerializeField] private Slider m_playerCapacity;
    [SerializeField] private Toggle m_hideRoomFromLobby;

    [SerializeField] private Text m_playerCapacityDisplayText; // for updating the text in CreateRoom

    [Separator("Joining a room")]
    [SerializeField] private InputField m_roomNameInput2;

    [Separator("Lobby / room text infomation")]
    [SerializeField] private Text m_statusFieldText;
    [SerializeField] private Text m_currentLocationText;

    [SerializeField] private Text m_roomVisibleText;
    [SerializeField] private Text m_roomCapacityText;

    [Separator("Buttons")]
    [SerializeField] private Button m_leaveRoomButton;
    [SerializeField] private Button m_startGameButton;
    [SerializeField] private Button m_createRoomButton;
    [SerializeField] private Button m_joinRoomButton;

    [Separator("The main windows/UI (to hide and show if needed)")]
    [SerializeField] private GameObject m_playersMaxPlayersText;
    [SerializeField] private GameObject m_joinButtonText;
    
    [SerializeField] private GameObject m_roomListGUI;
    [SerializeField] private GameObject m_playerListGUI;

    [SerializeField] private GameObject m_changeNameWindow;
    [SerializeField] private GameObject m_createGameWindow;
    [SerializeField] private GameObject m_joinGameWindow;

    private Coroutine m_createRoomButtonError;
    private Coroutine m_joinRoomButtonError;

    #endregion

    #region Monobehaviour callbacks

    private void Start()
    {
        Initialize();
        Connect();
    }

    #endregion

    #region Photon callbacks

    public override void OnCreatedRoom()
    {
        Debug.Log("Created a room successfully.");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Didn't create a room, see error: " + message);
    }

    public override void OnConnectedToMaster()
    {
        m_statusFieldText.text = "Connected to master server";
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        //Debug.LogWarningFormat("OnDisconnected() called by PUN with reason{0}", cause);

        if (!m_statusFieldText) { return; }
        m_statusFieldText.text = "Disconnected";
    }

    public override void OnJoinedLobby()
    {
        m_statusFieldText.text = "Connected to lobby";
        m_currentLocationText.text = "Rooms";
    }

    public override void OnJoinedRoom()
    {
        Room currentRoom = PhotonNetwork.CurrentRoom;

        // Set the text values
        m_statusFieldText.text = "Joined " + currentRoom.Name;
        m_currentLocationText.text = "Room: " + currentRoom.Name;

        if (currentRoom.IsVisible)
        {
            m_roomVisibleText.text = "This room is visible, people can join the room through the lobby.";
        }
        else
        {
            m_roomVisibleText.text = "This room is invisible, but people can still join this room if they have the room's name.";
        }

        if (currentRoom.MaxPlayers == 1)
        {
            m_roomCapacityText.text = "Room capacity: 1 player";
        }
        else
        {
            m_roomCapacityText.text = $"Room capacity: {currentRoom.MaxPlayers} players";
        }

        m_leaveRoomButton.gameObject.SetActive(true);
        m_startGameButton.gameObject.SetActive(true);
        m_leaveRoomButton.interactable = true;

        // If you are the master client, you can start the game
        if (PhotonNetwork.IsMasterClient)
        {
            m_startGameButton.interactable = true;
        }

        ShowWindow(false);
    }

    public override void OnLeftRoom()
    {
        if (m_statusFieldText)
        {
            m_statusFieldText.text = "Connecting...";
        }
        if (m_currentLocationText)
        {
            m_currentLocationText.text = "Rooms";
        }

        m_leaveRoomButton.gameObject.SetActive(false);
        m_startGameButton.gameObject.SetActive(false);
        m_startGameButton.interactable = false;

        ShowWindow(true);
    }

    #endregion

    #region UI callbacks

    /// <summary>
    /// Invoked by the create room button in the LobbyScene, creates a room, and the 
    /// OnRoomListUpdate would update the GUI to display a room (to other players,
    /// since if you created the room, you would already be in the room).
    /// </summary>
    public void OnClickCreateRoom()
    {
        // If not connected, don't create a room
        if (!PhotonNetwork.IsConnected)
        {
            ButtonErrorWrapper(m_createRoomButton, "Not connected to Photon servers");
            return;
        }

        // Prevent creating rooms with no name
        if (!string.IsNullOrEmpty(m_roomNameInput.text))
        {
            // add options for changing number of players
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = (byte)m_playerCapacity.value;

            options.IsVisible = !m_hideRoomFromLobby.isOn;

            PhotonNetwork.CreateRoom(m_roomNameInput.text, options, TypedLobby.Default);
        }
        else
        {
            ButtonErrorWrapper(m_createRoomButton, "Invalid room name");
        }
    }

    public void OnClickJoinRoom()
    {
        // If not connected, don't create a room
        if (!PhotonNetwork.IsConnected)
        {
            ButtonErrorWrapper(m_joinRoomButton, "Not connected to Photon servers");
            return;
        }

        string roomJoin = m_roomNameInput2.text;

        // Prevent joining rooms with no name
        if (string.IsNullOrEmpty(roomJoin))
        {
            ButtonErrorWrapper(m_joinRoomButton, "Invalid room name");
            return;
        }

        // See: https://forum.photonengine.com/discussion/17143/check-if-room-exists
        // for why there isn't a check to see if a room exists
        PhotonNetwork.JoinRoom(roomJoin);

        if (!PhotonNetwork.InRoom)
        {
            ButtonErrorWrapper(m_joinRoomButton, "Room doesn't exist or it's full");
        }
    }

    public void OnMainMenuButtonPressed()
    {
        PhotonNetwork.Disconnect(); // you need to disconnect when exiting the lobby
        SceneManager.LoadScene("MainMenu");
    }

    public void OnSliderUpdatedDisplayMaxPlayerCount(float maxPlayers)
    {
        m_playerCapacityDisplayText.text = "Max players: " + ((int)maxPlayers).ToString();
    }

    public void OnStartGamePressed()
    {
        PhotonNetwork.LoadLevel(sceneNameToLoadIn);
    }

    #endregion

    #region Private functions

    /// <summary>
    /// When you mess up, the button will be noninteractable, turn red, and tell you why.
    /// </summary>
    /// <param name="buttonToDisplayError">The button that will be noninteractable for the duration</param>
    /// <param name="errorMsg">Error message to display</param>
    /// <param name="durationDisplayError">Duration button is errored.</param>
    /// <returns></returns>
    private IEnumerator ButtonError(Button buttonToDisplayError, string errorMsg = "Error", float durationDisplayError = 1.5f)
    {
        buttonToDisplayError.interactable = false;

        Image imageSibling = buttonToDisplayError.gameObject.GetComponent<Image>(); // image component that is the sibling of the button component
        Text buttonText = buttonToDisplayError.transform.GetComponentInChildren<Text>();

        Color originalButtonColor = imageSibling.color;
        string originalMessage = buttonText.text;

        imageSibling.color = Color.red;
        buttonText.text = errorMsg;

        yield return new WaitForSecondsRealtime(durationDisplayError);

        imageSibling.color = originalButtonColor;
        buttonText.text = originalMessage;

        buttonToDisplayError.interactable = true;
    }

    /// <summary>
    /// Make sure only one button error exist at once.
    /// </summary>
    private void ButtonErrorWrapper(Button buttonToDisplayError, string errorMsg = "Error", float durationDisplayError = 1.5f)
    {
        if (buttonToDisplayError == m_createRoomButton)
        {
            if (m_createRoomButtonError != null)
            {
                StopCoroutine(m_createRoomButtonError);
            }
            m_createRoomButtonError = StartCoroutine(ButtonError(buttonToDisplayError, errorMsg, durationDisplayError));
        }
        else if (buttonToDisplayError == m_joinRoomButton)
        {
            if (m_joinRoomButtonError != null)
            {
                StopCoroutine(m_joinRoomButtonError);
            }
            m_joinRoomButtonError = StartCoroutine(ButtonError(buttonToDisplayError, errorMsg, durationDisplayError));
        }
    }

    /// <summary>
    /// The function connects the player to the Photon master server.
    /// </summary>
    private void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.AutomaticallySyncScene = true; // important
    }

    /// <summary>
    /// Do anything you need to do in start here.
    /// </summary>
    private void Initialize()
    {
        m_leaveRoomButton.interactable = false;
        m_startGameButton.interactable = false;
        m_leaveRoomButton.gameObject.SetActive(false);
        m_startGameButton.gameObject.SetActive(false);

        // Register the spells as a custom type
        PhotonPeer.RegisterType(typeof(RedOrb), (byte)'A', RedOrb.Serialize, RedOrb.Deserialize);
        PhotonPeer.RegisterType(typeof(OrangeOrb), (byte)'B', OrangeOrb.Serialize, OrangeOrb.Deserialize);
        PhotonPeer.RegisterType(typeof(YellowOrb), (byte)'C', YellowOrb.Serialize, YellowOrb.Deserialize);
        PhotonPeer.RegisterType(typeof(GreenOrb), (byte)'D', GreenOrb.Serialize, GreenOrb.Deserialize);
        PhotonPeer.RegisterType(typeof(BlueOrb), (byte)'E', BlueOrb.Serialize, BlueOrb.Deserialize);
        PhotonPeer.RegisterType(typeof(VioletOrb), (byte)'F', VioletOrb.Serialize, VioletOrb.Deserialize);
        PhotonPeer.RegisterType(typeof(BrownOrb), (byte)'G', BrownOrb.Serialize, BrownOrb.Deserialize);
        PhotonPeer.RegisterType(typeof(QuickSilverOrb), (byte)'H', QuickSilverOrb.Serialize, QuickSilverOrb.Deserialize);
        PhotonPeer.RegisterType(typeof(IndigoOrb), (byte)'I', IndigoOrb.Serialize, IndigoOrb.Deserialize);
    }

    /// <summary>
    /// If true, displays the roomList and hides the playerList, otherwise, do the inverse.
    /// </summary>
    /// <param name="isRoomList">Is the GUI we're displaying the room list?</param>
    private void ShowWindow(bool isRoomList)
    {
        m_playersMaxPlayersText.SetActive(isRoomList);
        m_joinButtonText.SetActive(isRoomList);
        m_roomListGUI.SetActive(isRoomList);

        m_createGameWindow.SetActive(isRoomList);
        m_changeNameWindow.SetActive(isRoomList);
        m_joinGameWindow.SetActive(isRoomList);

        m_roomVisibleText.gameObject.SetActive(!isRoomList);
        m_roomCapacityText.gameObject.SetActive(!isRoomList);

        m_playerListGUI.SetActive(!isRoomList);
    }

    #endregion

    #region Public functions

    /// <summary>
    /// Invoked when the masterclient left the room, and you're now the masterclient, you should be able to start the game.
    /// </summary>
    public void ActivateStartButton()
    {
        m_startGameButton.interactable = true;
    }

    /// <summary>
    /// Leaves a room and returns to the lobby view.
    /// </summary>
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(true);
    }

    #endregion
}
