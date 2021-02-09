using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region Public Fields

    [HideInInspector]
    public GameObject m_playerListFolder;

    #endregion

    #region Private Fields

    [Tooltip("The prefab to use for representing the player")]
    [SerializeField]
    private GameObject m_playerPrefab;

    [Tooltip("The prefab for the Health and Mana GUI")]
    [SerializeField]
    private GameObject m_healthManaBarPrefab;

    [Tooltip("The name of the folder that stores the player gameobjects")]
    [SerializeField]
    private string m_playerListFolderName = "PlayerList";

    private List<GameObject> m_playerGameObjectList; // list of player game object

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
    /// Returns a list of all the gameobjects associated with a player in the server.
    /// FetchPlayerGameObjects can be called with a string (username) to fetch the GameObject of a player
    /// If that player exists, returns that GameObject in a List, otherwise returns null
    /// </summary>
    /// <param name="specificPlayer"></param>
    /// <returns></returns>
    public List<GameObject> FetchPlayerGameObjects(string specificPlayer = "")
    {
        if (specificPlayer == "")
        {
            // O(n) search, remove all null GameObjects from list
            int ind = 0;
            while (ind < m_playerGameObjectList.Count)
            {
                GameObject tmp = m_playerGameObjectList[ind];
                if (!tmp)
                {
                    m_playerGameObjectList.Remove(tmp);
                }
                else
                {
                    ind += 1;
                }
            }
            return m_playerGameObjectList;
        }
        else
        {
            // search for player and return it, otherwise return null
            foreach (GameObject player in m_playerGameObjectList)
            {
                if (player.name == specificPlayer)
                {
                    return new List<GameObject> { player };
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Adds the player GameObject to the player GameObject list and also sets the player GameObject as a child of the playerlist folder
    /// </summary>
    /// <param name="name"></param>
    public void AddPlayerToGameObjectList(GameObject player)
    {
        player.transform.parent = m_playerListFolder.transform;
        m_playerGameObjectList.Add(player);
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

        m_playerGameObjectList = new List<GameObject>();
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
                    GameObject player = PhotonNetwork.Instantiate(m_playerPrefab.name, new Vector3(0f, 3f, 0f), Quaternion.identity, 0);

                    // Instantiate the health/mana GUI after instantiating the player
                    GameObject playerUI = Instantiate(m_healthManaBarPrefab);
                    playerUI.GetComponent<PlayerGUI>().SetTarget(player);
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
    }

    void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork::Trying to load level but not master");
        }
        PhotonNetwork.LoadLevel("SampleScene");
    }
    #endregion
}
