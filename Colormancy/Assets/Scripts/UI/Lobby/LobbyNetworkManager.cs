using System.Collections;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MyBox;
using System;

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

    [SerializeField] private Text m_regionPlayerCountText; // if connected to lobby, displays the current region you're connected to and the number of players online

    [Separator("Buttons / interactables")]
    [SerializeField] private Button m_leaveRoomButton;
    [SerializeField] private Button m_startGameButton;
    [SerializeField] private Button m_createRoomButton;
    [SerializeField] private Button m_joinRoomButton;
    [SerializeField] private Button m_returnMainMenuButton;

    [SerializeField] private Dropdown m_changeRegionDropdown;

    // these two below are the fake / real labels that display the region name
    // the fake label is only used when we load into the lobby for the first time
    // and it correctly displays the region with the best ping for the player
    // it'll go away if the dropdown is selected
    private GameObject m_fakeLabel;
    private GameObject m_realLabel;

    [Separator("The main windows/UI (to hide and show if needed)")]
    [SerializeField] private GameObject m_playersMaxPlayersText;
    [SerializeField] private GameObject m_joinButtonGUIText;

    [SerializeField] private GameObject m_roomListGUI;
    [SerializeField] private GameObject m_playerListGUI;

    [SerializeField] private GameObject m_changeNameWindow;
    [SerializeField] private GameObject m_createGameWindow;
    [SerializeField] private GameObject m_joinGameWindow;

    [SerializeField] private GameObject m_changeRegionWindow;

    private Coroutine m_createRoomButtonError;
    private Coroutine m_joinRoomButtonError;

    private Color m_buttonWhite;
    private Color m_buttonRed = Color.red;

    // the order of regions (in the array bellow) is the same order displayed in the change region dropdown
    // china is not available (see: https://doc.photonengine.com/en/pun/current/connection-and-authentication/regions#using_the_chinese_mainland_region)
    private string[] m_photonRegions = new string[] { "asia", "au", "cae", "eu", "in", "jp", "ru", "rue", "za", "sa", "kr", "us", "usw" };

    private bool m_loadingInFromMainMenu = true; // flag for denoting that we loaded from the main menu, and we haven't chosen the region yet (defaults to region with best ping

    [Separator("References to other scripts")]
    [SerializeField] private RoomListManager m_roomListManager;

    #endregion

    #region Monobehaviour callbacks

    private void Start()
    {
        m_buttonWhite = m_createRoomButton.GetComponent<Image>().color; // fetch the normal color used by the createRoomButton

        // fetch the labels for the change region dropdown
        m_fakeLabel = m_changeRegionDropdown.transform.Find("FalseLabel").gameObject;
        m_realLabel = m_changeRegionDropdown.transform.Find("RealLabel").gameObject;

        Initialize();
        Connect();
    }

    private void Update()
    {
        // always update the number of players online and the current region
        if (PhotonNetwork.InLobby || PhotonNetwork.InRoom)
        {
            // Always display this status as long as we're connected to either a lobby or a room
            string regionName = PhotonNetwork.CloudRegion;
            int index = regionName.IndexOf("/*");

            //if (index < 0)
            //{
            //    print("Region " + regionName + "doesn't have the suffix '/*'");
            //}
            //else
            //{
            //    print("Region " + regionName + "has the suffix '/*'");
            //}

            string cleanedRegion = (index < 0) ? regionName : regionName.Remove(index, regionName.Length - index);
            m_regionPlayerCountText.text = "Region: " + cleanedRegion + " | Players online in this region: " + PhotonNetwork.CountOfPlayers;
        }
        else
        {
            if (m_regionPlayerCountText.text != "Not connected to either a lobby or a room.")
            {
                m_regionPlayerCountText.text = "Not connected to either a lobby or a room.";
            }
        }

        if (!PhotonNetwork.InLobby)
        {
            // you can only change regions while you're in the lobby
            if (m_changeRegionDropdown.interactable)
            {
                m_changeRegionDropdown.interactable = false;
            }
        }
    }

    #endregion

    #region Photon callbacks

    public override void OnConnectedToMaster()
    {
        m_statusFieldText.text = "Connected to master server";
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        m_statusFieldText.text = "Failed to create room";
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

        // Update the region dropdown to show the fullname of the region
        if (m_loadingInFromMainMenu)
        {
            m_loadingInFromMainMenu = false;
            m_fakeLabel.GetComponent<Text>().text = m_changeRegionDropdown.options[Array.IndexOf(m_photonRegions, PhotonNetwork.CloudRegion)].text;

            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = PhotonNetwork.CloudRegion;

            // Initially we had set: PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "" when we loaded into from the menu
            // this found the best region with the best ping
            // However we need to disconnect and reconnect again because all of the regions we connect to from the dropdown menu
            // have a suffix of /*, this makes those regions distinct (i.e: "usw" and "usw/*" are distinct regions)
            // Having set the FixedRegion variable, reconnecting will place us in the correct region
            PhotonNetwork.Disconnect();
            PhotonNetwork.ConnectUsingSettings();
        }

        m_changeRegionDropdown.interactable = true;
    }

    public override void OnJoinedRoom()
    {
        Room currentRoom = PhotonNetwork.CurrentRoom;

        StopAllCoroutines(); // stop the coroutines for the create room and join room buttons displayed in the lobby

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

        // If you are the master client, you can start the game
        if (PhotonNetwork.IsMasterClient)
        {
            m_startGameButton.interactable = true;
        }

        ShowWindow(false);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        m_statusFieldText.text = "Failed to join room";
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
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

        m_createRoomButton.GetComponentInChildren<Text>().text = "Create room";
        m_createRoomButton.GetComponent<Image>().color = m_buttonWhite;

        m_joinRoomButton.GetComponentInChildren<Text>().text = "Join room";
        m_joinRoomButton.GetComponent<Image>().color = m_buttonWhite;

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

        // If not ready, don't create a room
        if (!PhotonNetwork.InLobby || !PhotonNetwork.IsConnectedAndReady)
        {
            ButtonErrorWrapper(m_createRoomButton, "Not ready to create room yet.");
            return;
        }

        string roomName = m_roomNameInput.text;

        if (string.IsNullOrEmpty(roomName))
        {
            roomName = PhotonNetwork.LocalPlayer.NickName + "'s Room"; // if room name is empty, it should be USERNAME's room by default
        }

        // add options for changing number of players
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)m_playerCapacity.value;

        options.IsVisible = !m_hideRoomFromLobby.isOn;

        PhotonNetwork.CreateRoom(roomName, options, TypedLobby.Default);

        StartCoroutine(CheckIfYouHaveCreatedRoom());
    }

    /// <summary>
    /// This callback is for when you click on the dropdown menu to change your region.
    /// </summary>
    /// <param name="newRegion">The region index you want to change to.</param>
    public void OnChangeRegion(int newRegion)
    {
        // Ignore the default dropdown (-1) and ignore changing to a region you're already in
        if (newRegion > -1 && newRegion != Array.IndexOf(m_photonRegions, PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion))
        {
            // it's important to disable the button while the player is changing regions, otherwise
            // complications may occur if a player decides to interact with the dropdown button again
            m_changeRegionDropdown.gameObject.SetActive(false);

            if (m_fakeLabel.activeSelf)
            {
                // Disable the fakeLabel because the value for dropdown won't be -1, the real one can take it from here
                m_fakeLabel.SetActive(false);
                m_realLabel.SetActive(true);
            }

            //print("switching to " + m_photonRegions[newRegion] + " from " + PhotonNetwork.CloudRegion);
            m_roomListManager.ClearRoomList();

            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = m_photonRegions[newRegion];
            PhotonNetwork.Disconnect();
            PhotonNetwork.ConnectUsingSettings();

            m_changeRegionDropdown.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// This callback is for the UI element with the input field (you can specify a room to join), not the join button located on the room UI elements.
    /// </summary>
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
            ButtonErrorWrapper(m_joinRoomButton, "Room name cannot be empty");
            return;
        }

        if (PhotonNetwork.InRoom)
        {
            ButtonErrorWrapper(m_joinRoomButton, "You're already in this room!");
            return;
        }

        // See: https://forum.photonengine.com/discussion/17143/check-if-room-exists
        // for why there isn't a check to see if a room exists
        PhotonNetwork.JoinRoom(roomJoin);

        StartCoroutine(CheckIfYouHaveJoinedTheRoom());
    }

    /// <summary>
    /// This function returns the player to the main menu after disconnecting from photon.
    /// </summary>
    public void OnMainMenuButtonPressed()
    {
        PhotonNetwork.Disconnect(); // you need to disconnect when exiting the lobby
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// This function updates the display for the player capacity in your personal room.
    /// </summary>
    /// <param name="maxPlayers">The desired player capacity (min is 1, max is 4)</param>
    public void OnSliderUpdatedDisplayMaxPlayerCount(float maxPlayers)
    {
        m_playerCapacityDisplayText.text = "Max players: " + ((int)maxPlayers).ToString();
    }

    /// <summary>
    /// This function starts the game, can only be invoked if you're the masterclient, and you've created a room.
    /// </summary>
    public void OnStartGamePressed()
    {
        m_startGameButton.interactable = false; // disable the start and leave room button to prevent race conditions
        m_leaveRoomButton.interactable = false;

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

        Image buttonImage = buttonToDisplayError.gameObject.GetComponent<Image>(); // image component that is the sibling of the button component
        Text buttonText = buttonToDisplayError.transform.GetComponentInChildren<Text>();

        string originalMessage = buttonText.text;

        buttonImage.color = m_buttonRed;
        buttonText.text = errorMsg;

        yield return new WaitForSecondsRealtime(durationDisplayError);

        buttonImage.color = m_buttonWhite;
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
    /// This is a simple 1 second delay wait and check.
    /// This makes the create room button non interactable for those two seconds and then makes it interactable again when it's done.
    /// </summary>
    private IEnumerator CheckIfYouHaveCreatedRoom()
    {
        m_createRoomButton.interactable = false;
        m_joinRoomButton.interactable = false; // prevent race condition
        
        Text buttonText = m_createRoomButton.GetComponentInChildren<Text>();
        string originalJoinButtonText = buttonText.text;
        buttonText.text = "Attempting to create room...";

        yield return new WaitForSecondsRealtime(1f);
        buttonText.text = originalJoinButtonText; // set the text back to the original before invoking the ButtonErrorWrapper
        m_createRoomButton.interactable = true;
        m_joinRoomButton.interactable = true;

        if (!PhotonNetwork.InRoom)
        {
            ButtonErrorWrapper(m_createRoomButton, "Room name already taken");
        }
    }

    /// <summary>
    /// This is a simple 1 second delay wait and check.
    /// This makes the join button non interactable for those two seconds and then makes it interactable again when it's done.
    /// </summary>
    private IEnumerator CheckIfYouHaveJoinedTheRoom()
    {
        m_joinRoomButton.interactable = false;
        m_createRoomButton.interactable = false; // prevent race condition

        Text buttonText = m_joinRoomButton.GetComponentInChildren<Text>();
        string originalJoinButtonText = buttonText.text;
        buttonText.text = "Attempting to join...";

        yield return new WaitForSecondsRealtime(1f);
        buttonText.text = originalJoinButtonText; // set the text back to the original before invoking the ButtonErrorWrapper
        m_joinRoomButton.interactable = true; // also make it interactable
        m_createRoomButton.interactable = true;

        if (!PhotonNetwork.InRoom)
        {
            ButtonErrorWrapper(m_joinRoomButton, "Room doesn't exist or it's full");
        }
    }

    /// <summary>
    /// The function connects the player to the Photon master server.
    /// </summary>
    private void Connect()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = ""; // coming in from the menu, find the best region with the best ping
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.AutomaticallySyncScene = true; // important
        }
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
        m_joinButtonGUIText.SetActive(isRoomList);
        m_roomListGUI.SetActive(isRoomList);

        m_createGameWindow.SetActive(isRoomList);
        m_changeNameWindow.SetActive(isRoomList);
        m_joinGameWindow.SetActive(isRoomList);
        m_changeRegionWindow.SetActive(isRoomList);

        m_returnMainMenuButton.interactable = isRoomList;
        m_createRoomButton.interactable = isRoomList;
        m_joinRoomButton.interactable = isRoomList;

        m_roomVisibleText.gameObject.SetActive(!isRoomList);
        m_roomCapacityText.gameObject.SetActive(!isRoomList);

        m_playerListGUI.SetActive(!isRoomList);

        m_leaveRoomButton.gameObject.SetActive(!isRoomList);
        m_startGameButton.gameObject.SetActive(!isRoomList);
        m_leaveRoomButton.interactable = !isRoomList;
        m_startGameButton.interactable = !isRoomList && PhotonNetwork.IsMasterClient;
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
        m_startGameButton.interactable = false;
        m_leaveRoomButton.interactable = false;
        PhotonNetwork.LeaveRoom(true);
    }

    #endregion
}
