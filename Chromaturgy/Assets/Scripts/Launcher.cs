using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Launcher : MonoBehaviourPunCallbacks
{
    #region Private Serializable Fields

    [Tooltip("Max number of players per room")]
    [SerializeField]
    private byte maxPlayersPerRoom = 4;

    [Tooltip("Panel where player can press play")]
    [SerializeField]
    private GameObject controlPanel;

    [Tooltip("Label that tells user they are connecting")]
    [SerializeField]
    private GameObject progressLabel;

    [Tooltip("Name of the scene to load after player joins a room")]
    [SerializeField]
    private string sceneNameToLoadIn = "Starting Level";

    #endregion

    #region Private Fields

    private string gameVersion = "1";

    private bool isConnecting;

    #endregion

    #region Unity Functions

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonPeer.RegisterType(typeof(RedOrb), (byte)'A', RedOrb.Serialize, RedOrb.Deserialize);
        PhotonPeer.RegisterType(typeof(OrangeOrb), (byte)'B', OrangeOrb.Serialize, OrangeOrb.Deserialize);
        PhotonPeer.RegisterType(typeof(YellowOrb), (byte)'C', YellowOrb.Serialize, YellowOrb.Deserialize);
        PhotonPeer.RegisterType(typeof(BlueOrb), (byte)'D', BlueOrb.Serialize, BlueOrb.Deserialize);
        PhotonPeer.RegisterType(typeof(VioletOrb), (byte)'E', VioletOrb.Serialize, VioletOrb.Deserialize);
        PhotonPeer.RegisterType(typeof(IndigoOrb), (byte)'F', IndigoOrb.Serialize, IndigoOrb.Deserialize);
    }

    // Start is called before the first frame update
    void Start()
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }

    #endregion

    #region Public Methods

    public void Connect()
    {
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);
        
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    #endregion

    #region MonoBehaviourPunCallbacks Callbacks

    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
            isConnecting = false;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
        Debug.LogWarningFormat("OnDisconnected() called by PUN with reason{0}", cause);
        isConnecting = false;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed()::no random room available");

        PhotonNetwork.CreateRoom(null, new RoomOptions{ MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.LoadLevel(sceneNameToLoadIn);
        }
    }

    #endregion
}
