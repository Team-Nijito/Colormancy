using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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
