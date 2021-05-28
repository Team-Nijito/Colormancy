using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable; // to use with Photon's CustomProperties

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region Public fields

    // Don't leave the m_levelType as None, set it to a value as soon as possible (when making new scenes)
    public enum LevelTypes
    {
        None,
        Level,
        Lobby,
        Narrative
    }

    // C# properties for accessing the private variables
    public LevelTypes TypeOfLevel { get { return m_levelType; } private set { m_levelType = value; } }
    public bool IsLevel { get { return !(m_levelType == LevelTypes.Lobby || m_levelType == LevelTypes.Narrative);  } }
    
    public int PlayersReady { get { return m_playersReady; } private set { m_playersReady = value; } }
    public uint PlayersNeededToReady { get { return m_playersNeededToStartGame; } private set { m_playersNeededToStartGame = value; } }
    public float PaintPercentageNeededToWin { get { return m_paintPercentageNeededToWin; } private set { m_paintPercentageNeededToWin = value; } }

    // Room custom properties
    public const string RedOrbKey = "RedLoanedToPhotonID";
    public const string OrangeOrbKey = "OrangeLoanedToPhotonID";
    public const string YellowOrbKey = "YellowLoanedToPhotonID";
    public const string GreenOrbKey = "GreenLoanedToPhotonID";
    public const string BlueOrbKey = "BlueLoanedToPhotonID";
    public const string VioletOrbKey = "VioletLoanedToPhotonID";
    public const string BrownOrbKey = "BrownLoanedToPhotonID";
    public const string QuicksilverOrbKey = "QuicksilverLoanedToPhotonID";
    public const string IndigoOrbKey = "IndigoLoanedToPhotonID";

    // Player custom properties
    public const string OrbOwnedInLobbyKey = "OrbOwned";
    public const string PlayerAliveKey = "IsPlayerAlive";

    // Name of scenes
    public const string LobbySceneName = "Starting Level";
    public const string WinSceneName = "YouWinScene";
    public const string OfficeLv1Name = "Office Level 1";
    #endregion

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

    private uint m_currentSpawnIndex = 0; // index of the current spawn to spawn the player, used if m_playerSpawnpoints exists

    // Ready up variables
    [SerializeField]
    private LevelTypes m_levelType = LevelTypes.None;

    private int m_playersReady = 0;

    [MyBox.ConditionalField(nameof(m_levelType), true, LevelTypes.Level)]
    [SerializeField]
    private uint m_playersNeededToStartGame = 5;

    [MyBox.ConditionalField(nameof(m_levelType), true, LevelTypes.Level)]
    [SerializeField]
    private string m_levelAfterReadyUp = OfficeLv1Name;

    private GameObject m_cameraPrefab; // Instantiate a camera prefab once a player returns back from a level

    [MyBox.ConditionalField(nameof(m_levelType), false, LevelTypes.Level)]
    [SerializeField]
    private string m_levelAfterBeatingStage = WinSceneName;

    private bool m_isLoadingNewScene = false;

    // Painting variables
    [Range(0, 1)]
    [SerializeField]
    private float m_paintPercentageNeededToWin = 0.75f;

    // Confiscate player orbs var
    private bool resetPlayerOrbs = false; // this flag tells us whenever a player lost and returns to the lobby, confiscate their orbs!

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
    PodiumController currentPodium; // keep track of current podium so we can close access to orb after player has obtained it
    OrbManager playerOrbManager;
    int dialoguePage = 0;

    #endregion

    #region Components

    [SerializeField]
    private EnemyManager m_enemManager;

    [SerializeField]
    private GameObject paintingManagerObject;
    [SerializeField]
    private GameObject orbValueManagerObject;

    #endregion

    #region MonoBehaviour callbacks

    private void Start()
    {
        if ((SceneManager.GetActiveScene().name == WinSceneName) || !PhotonNetwork.InRoom)
        {
            // don't run the rest of the start statement, return immediately
            return;
        }

        if (m_playerPrefab == null)
        {
            Debug.LogError("<Color=Red>Missing player prefab");
        }
        else
        {
            m_cameraPrefab = Resources.Load<GameObject>("Main Camera"); // load the camera resource before we might need it

            if (SceneManager.GetActiveScene().name == LobbySceneName)
            {
                // Initalize the Room's custom properties once for the starting level
                // be sure to clear these properties when moving to the first level

                PhotonHashtable properties;
                if (PhotonNetwork.IsMasterClient)
                {
                    // only initialize the Room custom properties once upon joining new scene

                    int[] array2size = new int[] { -1, -1 }; // first element, ID that is browsing the orb, second element, ID that has obtained the orb

                    // Essentially we're keeping track of who has which orb in the lobby
                    // we don't need to store a variable and sync the variable, rather
                    // we're delegating that task to Photon via CustomProperties (basically a HashTable)
                    properties = new PhotonHashtable
                    {
                        {RedOrbKey, array2size},
                        {OrangeOrbKey, array2size},
                        {YellowOrbKey, array2size},
                        {GreenOrbKey, array2size},
                        {BlueOrbKey, array2size},
                        {VioletOrbKey, array2size},
                        {BrownOrbKey, array2size},
                        {QuicksilverOrbKey, array2size},
                        {IndigoOrbKey, array2size}
                    };

                    PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
                }

                // always setup a custom properties for all player
                properties = new PhotonHashtable
                {
                    {OrbOwnedInLobbyKey, PodiumController.OrbTypes.None}
                };

                PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
                // these custom properties will be mutated whenver a player picks up a lobby orb / return a lobby orb
            }
            else if (m_levelType == LevelTypes.Level)
            {
                // Keep track of whether a player is still alive or not
                PhotonHashtable properties = new PhotonHashtable
                {
                    {PlayerAliveKey, true},
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
            }

            // Add in Painting Manager and Orb Value Manager classes manually
            if (!GameObject.Find("OrbValueManager"))
                Instantiate(orbValueManagerObject);
            if (!GameObject.Find("PaintingManager"))
                Instantiate(paintingManagerObject);

            if (PlayerController.LocalPlayerInstance == null)
            {
                if (HealthScript.LocalPlayerInstance == null)
                {
                    SpawnEntirelyNewPlayerAtSpawnpoint();
                }
                else
                {
                    //Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);

                    // This portion of the code is reached whenever HealthScript.LocalPlayerInstance exists
                    // meaning we're loading a new scene (player is Don't Destroy on load)

                    GameObject playerObject = PhotonNetwork.LocalPlayer.TagObject as GameObject;

                    if (!(m_levelType==LevelTypes.Level) && playerObject)
                    {
                        object playerAliveProperty;
                        bool spawnNewPlayerInstance = false;

                        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PlayerAliveKey, out playerAliveProperty))
                        {
                            if (!(bool)playerAliveProperty)
                            {
                                // the player is dead, remove any existing gameObject and spawn a new one

                                spawnNewPlayerInstance = true;

                                TidyUpBeforeStartingNewLevel();

                                // We need to instantiate a new main camera 
                                // because we've destroyed our previous one and the lobby doesn't have it anymore
                                Instantiate(m_cameraPrefab);

                                PhotonNetwork.Destroy(PhotonView.Get(playerObject));

                                SpawnEntirelyNewPlayerAtSpawnpoint();
                            }
                        }

                        if (!spawnNewPlayerInstance)
                        {
                            TransitionPlayerToNewRoom();
                        }
                    }
                    else
                    {
                        TransitionPlayerToNewRoom();
                    }
                }
            }
            else
            {
                //Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
        }

        SetPopUpVariables();
    }

    private void Update()
    {
        // Only confiscate player orbs for all players if players have just returned from a level and are all dead
        if (SceneManager.GetActiveScene().name == LobbySceneName && !resetPlayerOrbs)
        {
            resetPlayerOrbs = true;

            GameObject playerObj = PhotonNetwork.LocalPlayer.TagObject as GameObject;

            if (playerObj)
            {
                object playerAliveProperty;

                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PlayerAliveKey, out playerAliveProperty))
                {
                    if (!(bool)playerAliveProperty)
                    {
                        // the player is dead, remove all of their orbs
                        playerObj.GetComponent<OrbManager>().ResetOrbs();
                    }
                }
            }
            else
            {
                // playerObj not ready to check yet
                resetPlayerOrbs = false;
            }
        }

        if (PhotonNetwork.IsMasterClient)
        {
            if (!(m_levelType == LevelTypes.Level))
            {
                // check if all players are ready
                if (!m_isLoadingNewScene && m_playersReady >= m_playersNeededToStartGame)
                {
                    LoadLevel(m_levelAfterReadyUp);
                }
            }
            else
            {
                if (!m_isLoadingNewScene && PaintingManager.paintingProgress() > m_paintPercentageNeededToWin)
                {
                    LoadLevel(m_levelAfterBeatingStage);
                }
                if (!m_isLoadingNewScene)
                {
                    bool isAnyPlayerAlive = false;

                    foreach (Player p in PhotonNetwork.PlayerList)
                    {
                        object playerAliveProperty;
                        if (p.CustomProperties.TryGetValue(PlayerAliveKey, out playerAliveProperty))
                        {
                            if ((bool)playerAliveProperty)
                            {
                                isAnyPlayerAlive = true;
                            }
                        }
                        else
                        {
                            // race condition (need to load properties), players are still loading in
                            isAnyPlayerAlive = true;
                        }
                    }

                    if (!isAnyPlayerAlive)
                    {
                        LoadLevel(LobbySceneName);
                    }
                }
            }
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Map an Orb to a GameManager.[Orb]key string in order to utilize both room and player CustomProperties.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private string FetchOrbKey(Orb.Element type)
    {
        switch (type)
        {
            case Orb.Element.Wrath:
                return RedOrbKey;
            case Orb.Element.Fire:
                return OrangeOrbKey;
            case Orb.Element.Light:
                return YellowOrbKey;
            case Orb.Element.Nature:
                return GreenOrbKey;
            case Orb.Element.Water:
                return BlueOrbKey;
            case Orb.Element.Poison:
                return VioletOrbKey;
            case Orb.Element.Earth:
                return BrownOrbKey;
            case Orb.Element.Wind:
                return QuicksilverOrbKey;
            case Orb.Element.Darkness:
                return IndigoOrbKey;
            default:
                return "InvalidKey";
        }
    }

    private void SetPopUpVariables()
    {
        animator = popUpBox.GetComponent<Animator>();
        popUpFullText = popUpBox.transform.Find("FullText").GetComponent<TMPro.TMP_Text>();
        popUpImageText = popUpBox.transform.Find("ImageText").GetComponent<TMPro.TMP_Text>();
        dialogueHalfImage = popUpBox.transform.Find("HalfImage").GetComponent<Image>();
        dialogueFullImage = popUpBox.transform.Find("FullImage").GetComponent<Image>();
        nextButton = popUpBox.transform.Find("NextButton").gameObject;
        acceptButton = popUpBox.transform.Find("AcceptButton").gameObject;
        acceptButton.SetActive(false);
    }

    /// <summary>
    /// Create a new instance for the player and spawns them at a spawnpoint.
    /// </summary>
    private void SpawnEntirelyNewPlayerAtSpawnpoint()
    {
        // Determine the spawnpoint to spawn the player on for the first time
        m_currentSpawnIndex = (uint)(m_playerSpawnpoints.Length > 0 ? (PhotonNetwork.LocalPlayer.ActorNumber % m_playerSpawnpoints.Length) - 1 : 0);
        Quaternion spawnRotation = Quaternion.identity;
        Vector3 spawnPosition = ReturnSpawnpointPosition(ref spawnRotation);
        photonView.RPC("SpawnPlayer", PhotonNetwork.LocalPlayer, spawnPosition, spawnRotation);
    }

    [PunRPC]
    private void SpawnPlayer(Vector3 spawnPos, Quaternion spawnRot)
    {
        PhotonNetwork.Instantiate("Player/"+m_playerPrefab.name, spawnPos, spawnRot);
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

    /// <summary>
    /// Clean up everything as we load the character into the new level.
    /// </summary>
    private void TidyUpBeforeStartingNewLevel()
    {
        // Clear the custom properties as we transition to a new scene
        PhotonNetwork.CurrentRoom.CustomProperties.Clear(); // clear room's custom properties (will be called more than once)
        PhotonNetwork.LocalPlayer.CustomProperties.Clear(); // clear player's custom properties

        // Destroy the current camera because the player already has one
        Destroy(GameObject.Find("Main Camera"));
    }

    /// <summary>
    /// Given that the Player's TabObject (or instance) already exist, just reset the references
    /// to GUI and stuff, and set players at the level's spawnpoints so that we won't have to spawn
    /// an entirely new player instance
    /// </summary>
    private void TransitionPlayerToNewRoom()
    {
        PhotonView playerView = PhotonView.Get(HealthScript.LocalPlayerInstance);
        GameManager newLevelGameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        TidyUpBeforeStartingNewLevel();

        if (playerView.IsMine)
        {
            // Teleport only all players to the first spawn? (bug)
            playerView.RPC("RespawnPlayer", PhotonNetwork.LocalPlayer);

            GameObject player = playerView.gameObject;

            // IMPORTANT: reset the references to gui b/c old references won't work after scene load

            // Reset their health GUI
            player.GetComponent<SpawnGUI>().ResetUIAfterSceneLoad();

            // Reset their spell manager GUI reference
            player.GetComponent<SpellManager>().Initialization();

            // Reset their spell GUI reference
            player.GetComponent<OrbManager>().Initialization();

            // Set the character's speed depending on the level
            player.GetComponent<PlayerMovement>().SetSpeedDependingOnLevel(newLevelGameManager.IsLevel);
        }
    }

    #endregion

    #region Public Methods

    public void AddCurrentOrb()
    {
        if (currentOrbType != null)
        {
            playerOrbManager.AddSpellOrb(currentOrbType, true);

            // Update custom properties
            string orbKey = FetchOrbKey(currentOrbType.getElement());

            // fetch, alter, then set room custom properties
            PhotonHashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            int[] orbProperties = (int[])roomProperties[orbKey];
            roomProperties[orbKey] = new int[]{orbProperties[0],PhotonNetwork.LocalPlayer.ActorNumber};
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);

            // fetch, alter, then set player custom properties
            PhotonHashtable playerProperties = PhotonNetwork.LocalPlayer.CustomProperties;
            playerProperties[OrbOwnedInLobbyKey] = PodiumController.FetchOrbType(orbKey);
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);

            CloseWindow();
        }
    }
    
    /// <summary>
    /// Alters the accept button behaviour in the Popupdialogue canvas object 
    /// </summary>
    public void ChangeGUIMode(AcceptButtonHandler.AcceptMode newMode)
    {
        acceptButton.GetComponent<AcceptButtonHandler>().ChangeCurrentMode(newMode);
    }

    /// <summary>
    /// Wrapper function to call CloseWindowVisually() and currentPodium.CloseWindow()
    /// </summary>
    public void CloseWindow()
    {
        CloseWindowVisually();
        if (currentPodium)
            currentPodium.CloseWindow();
    }

    public void CloseWindowVisually()
    {
        if (WindowOpen)
        {
            animator.SetTrigger("close");
            WindowOpen = false;
            playerOrbManager = null;
            currentOrbType = null;
            acceptButton.SetActive(false);
            nextButton.SetActive(true);
        }
    }

    /// <summary>
    /// Invoked by the Canvas button "Leave Room"
    /// </summary>
    public void LeaveRoom()
    {
        OrbManager.orbHistory.Clear(); // don't retain memory of spells after leaving game

        if (SceneManager.GetActiveScene().name == LobbySceneName)
        {
            // why would a bozo claim an orb and then leave, you're making me do this stupid edge case
            PodiumController.OrbTypes orbOwned = (PodiumController.OrbTypes)PhotonNetwork.LocalPlayer.CustomProperties[OrbOwnedInLobbyKey];
            if (orbOwned != PodiumController.OrbTypes.None)
            {
                // we must return this back to the source
                PhotonHashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
                string key = PodiumController.FetchOrbKey(orbOwned);
                roomProperties[key] = new int[] { -1, -1 }; // nobody should own the orb anymore

                PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);

                roomProperties = new PhotonHashtable();
                PhotonNetwork.LocalPlayer.SetCustomProperties(roomProperties); // clear the character's properties
            }
        }

        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
    }

    /// <summary>
    /// Load a new level and transition everyone from the current scene to the new level immediately.
    /// Guarentees that the level will only be loaded once if this function is invoked more than once.
    /// </summary>
    /// <param name="nameOfScene">The name of the scene to load</param>
    public void LoadLevel(string nameOfScene)
    {
        if (!m_isLoadingNewScene)
        {
            m_isLoadingNewScene = true;

            PhotonNetwork.LoadLevel(nameOfScene);        
        }
    }


    public void NextPage()
    {
        dialoguePage++;
        SetPage();
    }

    public void PodiumPopUp(string[] messages, Sprite[] images, Orb orbType, OrbManager orbManager, PodiumController orbPodiumScript)
    {
        if (!WindowOpen)
        {
            currentPodium = orbPodiumScript;
            currentOrbType = orbType;
            playerOrbManager = orbManager;

            dialogueMessages = messages;
            dialogueImages = images;
            dialoguePage = 0;

            animator.SetTrigger("pop");
            PodiumMessage = true;
            WindowOpen = true;

            SetPage();
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

    public void RemoveCurrentOrb()
    {
        if (currentOrbType != null)
        {
            playerOrbManager.RemoveSpellOrb(currentOrbType, true);

            // Update custom properties
            string orbKey = FetchOrbKey(currentOrbType.getElement());

            // fetch, alter, then set room custom properties
            PhotonHashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            int[] orbProperties = (int[])roomProperties[orbKey];
            roomProperties[orbKey] = new int[] { orbProperties[0], -1 };
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);

            // fetch, alter, then set player custom properties
            PhotonHashtable playerProperties = PhotonNetwork.LocalPlayer.CustomProperties;
            playerProperties[OrbOwnedInLobbyKey] = PodiumController.OrbTypes.None;
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);

            CloseWindowVisually();
            currentPodium.CloseWindow();
        }
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

    public void SetFullImage()
    {
        popUpFullText.gameObject.SetActive(false);
        dialogueFullImage.gameObject.SetActive(true);
        dialogueHalfImage.gameObject.SetActive(false);
        popUpImageText.gameObject.SetActive(false);

        dialogueFullImage.sprite = dialogueImages[dialoguePage];
    }

    public void SetFullText()
    {
        popUpFullText.gameObject.SetActive(true);
        dialogueHalfImage.gameObject.SetActive(false);
        dialogueFullImage.gameObject.SetActive(false);
        popUpImageText.gameObject.SetActive(false);

        popUpFullText.text = dialogueMessages[dialoguePage];
    }

    public void SetImageText()
    {
        popUpFullText.gameObject.SetActive(false);
        dialogueHalfImage.gameObject.SetActive(true);
        popUpImageText.gameObject.SetActive(true);
        dialogueFullImage.gameObject.SetActive(false);

        dialogueHalfImage.sprite = dialogueImages[dialoguePage];
        popUpImageText.text = dialogueMessages[dialoguePage];
    }

    public void SetPage()
    {
        if (dialogueImages.Length <= dialoguePage || dialogueImages[dialoguePage] == null)
        {
            //No Images
            if (dialogueMessages.Length <= dialoguePage || dialogueMessages[dialoguePage] == "")
            {
                //No Messages
                Debug.LogError("Went past given messages/images");
            }
            else
            {
                //Messages
                SetFullText();
            }
        }
        else
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

    #endregion

    #region Photon Methods

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    #endregion

    #region Photon functions

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!(m_levelType == LevelTypes.Level))
        {
            // Synchronize the number of players ready across all clients
            if (stream.IsWriting)
            {
                stream.SendNext(m_playersReady);
            }
            else
            {
                m_playersReady = (int)stream.ReceiveNext();
            }
        }
        else
        {
            // Synchronize the number of enemies across all clients
            if (m_enemManager)
            {
                // If enemManager doesn't exist, don't sync anything.
                if (stream.IsWriting)
                {
                    stream.SendNext(m_enemManager.CurrentNumberEnemiesInLevel);
                }
                else
                {
                    m_enemManager.SetNumEnemiesOnField((byte)stream.ReceiveNext());
                }
            }
        }
    }

    #endregion
}