using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviourPunCallbacks
{
    #region Accessors (c# Properties)
    
    public byte CurrentNumberEnemiesInLevel { get { return m_numEnemiesOnField; } private set { m_numEnemiesOnField = value; } }

    #endregion

    #region Private Fields

    [SerializeField]
    private bool m_enemySpawnPointsActive = false;

    // conditional field not compatable with arrays
    [SerializeField]
    private GameObject[] m_spawnpoints;
    [SerializeField]
    private GameObject m_levelBossSpawnpoint;

    [SerializeField]
    private GameObject[] m_enemyEntities;

    [MyBox.ConditionalField(nameof(m_enemySpawnPointsActive), false)]
    [SerializeField]
    private GameObject m_enemyFolder; // the folder the enemies will be organized under

    [SerializeField]
    private byte m_enemiesPerPlayer = 8; // 8 enemies per player, if there are 4 players, there'd be 32 enemies

    private byte m_desiredEnemiesOnField = 7; // how many enemies are on the field at the time

    private byte m_numEnemiesOnField = 0;

    #endregion

    #region Monobehaviour callbacks

    private void Start()
    {
        UpdateDesiredEnemiesInRoom();
    }

    private void Update()
    {
        if (m_enemySpawnPointsActive && PhotonNetwork.IsMasterClient)
        {
            // currently have problem for spawning in more enemies for each player
            if (m_numEnemiesOnField < m_desiredEnemiesOnField)
            {
                // Assume that we'll spawn an enemy successfully and increment numEnemiesOnField to prevent
                // a race condition (many enemies spawning at once)

                // If anything goes wrong when spawning the enemy, decrement numEnemiesOnField
                m_numEnemiesOnField++;
                TrySpawnEnemy(4f); 
            }
        }

        //if (m_enemySpawnPointsActive)
        //{
        //    print("Max enemies on field " + m_desiredEnemiesOnField);
        //}
    }

    #endregion

    #region Private functions

    private void TrySpawnEnemy(float delay = 0f)
    {
        GameObject entity = ChooseEntityForSpawning();

        // store the components of "Good" spawnpoints, where "Good" means an empty spawnpoint
        // or a spawnpoint where a enemy was just spawnkilled, and now it should be empty
        List<SpawnpointBehaviour> goodSpawnpointScripts = new List<SpawnpointBehaviour>();

        foreach (GameObject spawn in m_spawnpoints)
        {
            if (spawn.activeInHierarchy)
            {
                SpawnpointBehaviour spawnScript = spawn.GetComponent<SpawnpointBehaviour>();
                if (!spawnScript.IsSpawningEnemy && spawnScript.IsSpawnSafe())
                {
                    goodSpawnpointScripts.Add(spawnScript);
                }
            }
        }

        if (goodSpawnpointScripts.Count == 0)
        {
            //Debug.Log("No unobstructed spawnpoints to spawn on, waiting");
            m_numEnemiesOnField--;
        }
        else
        {
            // choose a random good spawnpoint
            SpawnpointBehaviour chosenScript = goodSpawnpointScripts[Random.Range(0, goodSpawnpointScripts.Count)];

            chosenScript.HandleSpawning(m_enemyFolder, entity.name, delay);
        }
    }

    /// <summary>
    /// Chooses an entity to spawn.
    /// </summary>
    /// <returns>The GameObject associated with the entity</returns>
    private GameObject ChooseEntityForSpawning()
    {
        // Randomly choose an enemy to spawn
        return m_enemyEntities[Random.Range(0, m_enemyEntities.Length)];
    }

    /// <summary>
    /// Invoked when the scene starts, when a player join or leaves, so that the desired enemies on the field would be updated.
    /// If there are 20 enemies (for 2 players), and a player leaves, those extra 10 enemies will still be on the field,
    /// but after they are slain, there should only be 10 enemies on the field.
    /// </summary>
    /// <returns></returns>
    private void UpdateDesiredEnemiesInRoom()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            m_desiredEnemiesOnField = (byte)(PhotonNetwork.CurrentRoom.PlayerCount * m_enemiesPerPlayer);
        }
    }

    #endregion

    #region Public functions

    /// <summary>
    /// The manager is notified that the number of enemies on the field is decreased. This should be invoked by the masterclient whenever
    /// an enemy entity has died.
    /// </summary>
    public void EnemyHasDied()
    {
        if (m_numEnemiesOnField > 0)
        {
            m_numEnemiesOnField--;
        }
    }

    /// <summary>
    /// Public setter function, for use in OnPhotonSerializeView in GameManager.cs.
    /// I wanted to avoid having an extra PhotonView in the scene, so I'd rather the GameManager
    /// handle the syncing for the number of enemies on the field.
    /// </summary>
    /// <param name="numEnemies"></param>
    public void SetNumEnemiesOnField(byte numEnemies)
    {
        m_numEnemiesOnField = numEnemies;
    }

    public GameObject SummonLevelBoss(Vector3 pos)
    {
        return m_levelBossSpawnpoint.GetComponent<SpawnpointBehaviour>().SpawnBoss(m_enemyFolder, "Diane, Doer of Deeds");
    }

    #endregion

    #region Photon functions

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateDesiredEnemiesInRoom();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateDesiredEnemiesInRoom();
    }

    public void SetEnemySpawnActive(bool val)
    {
        m_enemySpawnPointsActive = val;
    }

    #endregion
}
