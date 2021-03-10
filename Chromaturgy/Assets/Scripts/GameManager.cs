using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region Public Fields

    [HideInInspector]
    public GameObject m_playerListFolder;

    #endregion

    #region Private Fields

    [Tooltip("Players will spawn on these location(s)")]
    [SerializeField]
    private GameObject[] m_playerSpawnpoints;

    [Tooltip("If playerSpawnpoint is unassigned, spawn using these default coordinates")]
    [SerializeField]
    private Vector3 m_defaultSpawn = new Vector3(0, 3, 0);

    [Tooltip("The prefab to use for representing the player")]
    [SerializeField]
    private GameObject m_playerPrefab;

    [Tooltip("The prefab for the Health and Mana GUI")]
    [SerializeField]
    private GameObject m_healthManaBarPrefab;

    [Tooltip("The name of the folder that stores the player gameobjects")]
    [SerializeField]
    private string m_playerListFolderName = "PlayerList";

    private int m_currentSpawnIndex = 0; // index of the current spawn to spawn the player, used if m_playerSpawnpoints exists
    
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
        if (m_playerSpawnpoints.Length > 0)
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

            SetPage();

            animator.SetTrigger("pop");
            PodiumMessage = false;
            WindowOpen = true;
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

    #region Private Methods

    private void Awake()
    {
        m_playerListFolder = GameObject.Find(m_playerListFolderName);
        if (!m_playerListFolder)
        {
            m_playerListFolder = new GameObject(m_playerListFolderName);
        }
    }

    private void Start()
    {
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
                    m_currentSpawnIndex = m_playerSpawnpoints.Length > 0 ? (PhotonNetwork.LocalPlayer.ActorNumber % m_playerSpawnpoints.Length) - 1 : 0;
                    Quaternion spawnRotation = Quaternion.identity;
                    Vector3 spawnPosition = ReturnSpawnpointPosition(ref spawnRotation);
                    photonView.RPC("SpawnPlayer", PhotonNetwork.LocalPlayer, spawnPosition, spawnRotation);
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
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

    #endregion
}
