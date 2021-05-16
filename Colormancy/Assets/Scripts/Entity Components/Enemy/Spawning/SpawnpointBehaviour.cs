using Photon.Pun;
using UnityEngine;

public class SpawnpointBehaviour : MonoBehaviour
{
    // script for individual spawnpoints
    [SerializeField]
    private GameObject m_spawnPointHitbox;

    private GameObject m_entityOnSpawnPoint;

    [SerializeField]
    private bool m_spawnCustomEnemy = false;

    [SerializeField]
    [MyBox.ConditionalField(nameof(m_spawnCustomEnemy))]
    private GameObject m_enemyToSpawn;

    void OnTriggerEnter(Collider collision)
    {
        if (IsEntity(collision))
        {
            m_entityOnSpawnPoint = collision.gameObject;
        }
    }

    void OnTriggerExit(Collider collision)
    {
        if (IsEntity(collision))
        {
            m_entityOnSpawnPoint = null;
        }
    }

    private bool IsEntity(Collider collision)
    {
        return collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy");
    }
    
    
    public bool IsSpawnSafe(GameObject entityToSpawn)
    {
        // hmm spawn checking broke for now, just return true
        return true; // !m_entityOnSpawnPoint || m_entityOnSpawnPoint == entityToSpawn;
    }

    // spawn the enemy
    public void HandleSpawning(GameObject parentFolder, string nameEntityToSpawn)
    {
        string enemyName = "Enemies/";

        if (m_spawnCustomEnemy && m_enemyToSpawn)
        {
            enemyName += m_enemyToSpawn.name;
        }
        else
        {
            enemyName += nameEntityToSpawn;
        }

        GameObject entity = PhotonNetwork.InstantiateRoomObject(enemyName, 
                                                      m_spawnPointHitbox.transform.position, 
                                                      m_spawnPointHitbox.transform.rotation * Quaternion.Euler(0, 180f, 0), 
                                                      0);
        m_entityOnSpawnPoint = entity;
        entity.transform.parent = parentFolder.transform; // set the entity as a child of the parentFolder, for organizational purposes
    }
}
