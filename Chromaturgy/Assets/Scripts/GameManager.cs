using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region Private Fields

    // General variables

    [Tooltip("Players will spawn on these location(s)")]
    [SerializeField]
    private GameObject[] m_playerSpawnpoints;

    [Tooltip("If playerSpawnpoint is unassigned, spawn using these default coordinates")]
    [SerializeField]
    private Vector3 m_defaultSpawn = new Vector3(0, 3, 0);

    [Tooltip("The prefab to use for representing the player")]
    [SerializeField]
    private GameObject m_playerPrefab;

    public bool DoSpawnPlayer { get { return m_doSpawnPlayer; } private set { m_doSpawnPlayer = value; } }

    [Tooltip("Do we spawn players in this scene?")]
    [SerializeField]
    private bool m_doSpawnPlayer = true;

    private uint m_currentSpawnIndex = 0; // index of the current spawn to spawn the player, used if m_playerSpawnpoints exists

    // Ready up variables

    public bool IsLevel { get { return m_isLevel; } private set { m_isLevel = value; } }
    [SerializeField]
    private bool m_isLevel = true;

    public int PlayersReady { get { return m_playersReady; } private set { m_playersReady = value; } }
    public uint PlayersNeededToReady { get { return m_playersNeededToStartGame; } private set { m_playersNeededToStartGame = value; } }

    private int m_playersReady = 0;

    [SerializeField]
    private uint m_playersNeededToStartGame = 5;

    [SerializeField]
    private string m_levelAfterLobbyLevel = "Office Level 1";

    private bool m_isLoadingNewScene = false;

    // Painting variables

    public float PaintPercentageNeededToWin { get { return m_paintPercentageNeededToWin; } private set { m_paintPercentageNeededToWin = value; } }
    [Range(0,1)]
    [SerializeField]
    private float m_paintPercentageNeededToWin = 0.75f;

    [SerializeField]
    private string m_levelAfterBeatingStage = "YouWinScene";

    #endregion

    #region Dialogue system fields

    public GameObject popUpBox;

    Animator animator;
    TMPro.TMP_Text popUpFullText;
    TMPro.TMP_Text popUpImageText;
    Image dialogueHalfImage;
    Image dialogueFullImage;
    GameObject nextButton;
    GameObject acceptButton;

    Sprite[] dialogueImages;
    string[] dialogueMessages;

    bool WindowOpen = false;
    bool PodiumMessage = false;

    Orb currentOrbType;
    SpellTest playerSpellTest;

    int dialoguePage = 0;

    #endregion

    #region Photon Methods

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    #endregion

    #region Public Methods

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    /// <summary>
    /// Get the position of the spawnpoint (if it exists), otherwise return default
    /// </summary>
    /// <param name="spawnRotation">The rotation of the spawn, pass a quaternion by reference and it will be updated with this variable.</param>
    /// <returns>The position of the spawnpoint</returns>
    public Vector3 ReturnSpawnpointPosition(ref Quaternion spawnRotation)
    {
        if (m_playerSpawnpoints.Length > 0 && m_currentSpawnIndex < PhotonNetwork.PlayerList.Length)
        {
            Vector3 spawnPosition = m_playerSpawnpoints[m_currentSpawnIndex].transform.position;
            spawnRotation = m_playerSpawnpoints[m_currentSpawnIndex].transform.rotation;
            return spawnPosition;
        }
        else
        {
            return m_defaultSpawn;
        }
    }

    public void PopUp(string[] messages, Sprite[] images)
    {
        if (!WindowOpen)
        {
            dialogueMessages = messages;
            dialogueImages = images;
            dialoguePage = 0;

            animator.SetTrigger("pop");
            PodiumMessage = false;
            WindowOpen = true;

            SetPage();
        }
    }

    public void PodiumPopUp(string[] messages, Sprite[] images, Orb orbType, SpellTest spellTest)
    {
        if (!WindowOpen)
        {
            currentOrbType = orbType;
            playerSpellTest = spellTest;

            dialogueMessages = messages;
            dialogueImages = images;
            dialoguePage = 0;

            animator.SetTrigger("pop");
            PodiumMessage = true;
            WindowOpen = true;

            SetPage();
        }
    }

    public void NextPage()
    {
        dialoguePage++;
        SetPage();
    }

    void SetPage()
    {
        if (dialogueImages.Length <= dialoguePage || dialogueImages[dialoguePage] == null)
        {
            //No Images
            if (dialogueMessages.Length <= dialoguePage || dialogueMessages[dialoguePage] == "")
            {
                //No Messages
                Debug.LogError("Went past given messages/images");
            } else
            {
                //Messages
                SetFullText();
            }
        } else
        {
            //Images
            if (dialogueMessages.Length <= dialoguePage || dialogueMessages[dialoguePage] == "")
            {
                //No Messages
                SetFullImage();
            }
            else
            {
                //Messages
                SetImageText();
            }
        }

        if (dialoguePage == (Mathf.Max(dialogueImages.Length, dialogueMessages.Length) - 1))
        {
            nextButton.SetActive(false);
            if (PodiumMessage)
            {
                acceptButton.SetActive(true);
            }
        }
    }

    public void AddCurrentOrb()
    {
        if (currentOrbType != null)
        {
            playerSpellTest.AddSpellOrb(currentOrbType);
            CloseWindow();
        }
    }

    public void CloseWindow()
    {
        if (WindowOpen)
        {
            animator.SetTrigger("close");
            WindowOpen = false;
            playerSpellTest = null;
            currentOrbType = null;
            acceptButton.SetActive(false);
            nextButton.SetActive(true);
        }
    }

    void SetImageText()
    {
        popUpFullText.gameObject.SetActive(false);
        dialogueHalfImage.gameObject.SetActive(true);
        popUpImageText.gameObject.SetActive(true);
        dialogueFullImage.gameObject.SetActive(false);

        dialogueHalfImage.sprite = dialogueImages[dialoguePage];
        popUpImageText.text = dialogueMessages[dialoguePage];
    }

    void SetFullText()
    {
        popUpFullText.gameObject.SetActive(true);
        dialogueHalfImage.gameObject.SetActive(false);
        dialogueFullImage.gameObject.SetActive(false);
        popUpImageText.gameObject.SetActive(false);

        popUpFullText.text = dialogueMessages[dialoguePage];
    }

    void SetFullImage()
    {
        popUpFullText.gameObject.SetActive(false);
        dialogueFullImage.gameObject.SetActive(true);
        dialogueHalfImage.gameObject.SetActive(false);
        popUpImageText.gameObject.SetActive(false);

        dialogueFullImage.sprite = dialogueImages[dialoguePage];
    }

    #endregion

    #region MonoBehaviour callbacks

    private void Start()
    {
        if (!m_doSpawnPlayer || !PhotonNetwork.InRoom)
        {
            // don't do anything
            return;
        }

        if (m_playerPrefab == null)
        {
            Debug.LogError("<Color=Red>Missing player prefab");
        }
        else
        {
            if (PlayerController.LocalPlayerInstance == null)
            {
                if (HealthScript.LocalPlayerInstance == null)
                {
                    // Determine the spawnpoint to spawn the player on
                    m_currentSpawnIndex = (uint)(m_playerSpawnpoints.Length > 0 ? (PhotonNetwork.LocalPlayer.ActorNumber % m_playerSpawnpoints.Length) - 1 : 0);
                    Quaternion spawnRotation = Quaternion.identity;
                    Vector3 spawnPosition = ReturnSpawnpointPosition(ref spawnRotation);
                    photonView.RPC("SpawnPlayer", PhotonNetwork.LocalPlayer, spawnPosition, spawnRotation);
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);

                    // This portion of the code is reached whenever HealthScript.LocalPlayerInstance exists
                    // meaning we're loading a new scene (player is Don't Destroy on load)

                    PhotonView playerView = PhotonView.Get(HealthScript.LocalPlayerInstance);
                    
                    // Fetch the current scene's GameManager
                    GameManager m_currentGm = GameObject.Find("GameManager").GetComponent<GameManager>();

                    if (m_currentGm.DoSpawnPlayer && playerView.IsMine)
                    {
                        // Destroy the current camera because the player already has one
                        Destroy(GameObject.Find("Main Camera"));

                        // Teleport only all players to the first spawn? (bug)
                        playerView.RPC("RespawnPlayer", PhotonNetwork.LocalPlayer, true);

                        // Reset their GUI
                        playerView.gameObject.GetComponent<SpawnGUI>().ResetUIAfterSceneLoad();
                    }

                    // how do I spawn players at their spawnpoints? hmm..
                }
            }
            else
            {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
        }


        animator = popUpBox.GetComponent<Animator>();
        popUpFullText = popUpBox.transform.Find("FullText").GetComponent<TMPro.TMP_Text>();
        popUpImageText = popUpBox.transform.Find("ImageText").GetComponent<TMPro.TMP_Text>();
        dialogueHalfImage = popUpBox.transform.Find("HalfImage").GetComponent<Image>();
        dialogueFullImage = popUpBox.transform.Find("FullImage").GetComponent<Image>();
        nextButton = popUpBox.transform.Find("NextButton").gameObject;
        acceptButton = popUpBox.transform.Find("AcceptButton").gameObject;
        acceptButton.SetActive(false);
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (!m_isLevel)
            {
                // check if all players are ready
                if (m_playersReady >= m_playersNeededToStartGame)
                {
                    LoadFirstLevel();
                }
            }
            else
            {
                if (PaintingManager.paintingProgress() > m_paintPercentageNeededToWin)
                {
                    LoadNewSceneAfterFinishedPainting();
                }
            }
        }
    }

    #endregion

    #region Private Methods

    void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork::Trying to load level but not master");
        }
        PhotonNetwork.LoadLevel("SampleScene");
    }
    
    [PunRPC]
    private void SpawnPlayer(Vector3 spawnPos, Quaternion spawnRot)
    {
        PhotonNetwork.Instantiate(m_playerPrefab.name, spawnPos, spawnRot);
    }

    private void LoadFirstLevel()
    {
        if (!m_isLoadingNewScene)
        {
            m_isLoadingNewScene = true;

            // do it instantly for now
            PhotonNetwork.LoadLevel(m_levelAfterLobbyLevel);
        }
    }
    
    [PunRPC]
    private void ReadyUp()
    {
        m_playersReady++;
    }

    [PunRPC]
    private void UnReady()
    {
        m_playersReady--;
    }

    #endregion

    #region Public Methods

    public void LoadNewSceneAfterFinishedPainting()
    {
        if (!m_isLoadingNewScene)
        {
            m_isLoadingNewScene = true;

            // do it instantly for now
            PhotonNetwork.LoadLevel(m_levelAfterBeatingStage);
        }
    }

    /// <summary>
    /// Player readies up. Used for the lobby.
    /// </summary>
    public void RPCReadyUp()
    {
        photonView.RPC("ReadyUp", RpcTarget.All);
    }

    /// <summary>
    /// Player unreadies. Used for the lobby.
    /// </summary>
    public void RPCUnready()
    {
        photonView.RPC("UnReady", RpcTarget.All);
    }

    #endregion

    #region Photon functions

    // Synchronize the number of players ready across all clients
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!m_isLevel)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(m_playersReady);
            }
            else
            {
                m_playersReady = (int)stream.ReceiveNext();
            }
        }
    }

    #endregion
}
