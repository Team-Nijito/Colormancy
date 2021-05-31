using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviourPun
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
    private GameObject[] m_enemyEntities;

    [MyBox.ConditionalField(nameof(m_enemySpawnPointsActive), false)]
    [SerializeField]
    private GameObject m_enemyFolder; // the folder the enemies will be organized under

    [MyBox.ConditionalField(nameof(m_enemySpawnPointsActive), false)]
    [SerializeField]
    private byte m_desiredEnemiesOnField = 7; // how many enemies are on the field at the time

    private byte m_numEnemiesOnField = 0;

    #endregion

    #region Monobehaviour callbacks

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

    #endregion
}
