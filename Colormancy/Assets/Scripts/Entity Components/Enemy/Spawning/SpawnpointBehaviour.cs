using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpawnpointBehaviour : MonoBehaviour, IPunObservable
{
    #region Accessors (C# properties)

    // script for individual spawnpoints
    public bool IsSpawningEnemy { get { return m_isCurrentlySpawningEnemy; } private set { m_isCurrentlySpawningEnemy = value; } }

    #endregion

    #region Private fields

    [SerializeField]
    private GameObject m_spawnPointTrigger; // the trigger for the spawn, spawn the entity in the middle of this trigger 

    private GameObject m_spawnedEntity; 

    private float m_spawnpointRadius;

    [SerializeField]
    private bool m_spawnCustomEnemy = false;

    [SerializeField]
    [MyBox.ConditionalField(nameof(m_spawnCustomEnemy))]
    private GameObject m_enemyToSpawn;

    private bool m_isCurrentlySpawningEnemy = false;

    // Spawn delay and GUI
    private float m_timeUntilSpawnOriginal = 0f; 
    private float m_timeUntilSpawn = 0f; // count down to zero

    [SerializeField]
    private Image m_radialSpawnTimer;

    private LayerMask m_playerLayer;
    private LayerMask m_enemyLayer;

    #endregion

    #region Monobehaviour callbacks

    private void Start()
    {
        m_playerLayer = LayerMask.NameToLayer("Ignore Player");
        m_enemyLayer = LayerMask.NameToLayer("Enemy");

        BoxCollider triggerBox = m_spawnPointTrigger.GetComponent<BoxCollider>();
        m_spawnpointRadius = (triggerBox.size.x + triggerBox.size.z) * 0.5f; // take the average of x and z, incase they're different
    }

    private void Update()
    {
        if (m_spawnedEntity)
        {
            // If the spawnEntity got off the spawn, we don't care about the entity anymore
            if (Vector3.Distance(m_spawnedEntity.transform.position, m_spawnPointTrigger.transform.position) > m_spawnpointRadius)
            {
                m_spawnedEntity = null;
            }
        }

        if (m_timeUntilSpawn > 0f)
        {
            m_timeUntilSpawn -= Time.deltaTime;
            m_radialSpawnTimer.fillAmount = m_timeUntilSpawn / m_timeUntilSpawnOriginal;
        }
        else
        {
            if (m_radialSpawnTimer.gameObject.activeInHierarchy)
            {
                m_radialSpawnTimer.gameObject.SetActive(false);
            }
        }
    }

    #endregion

    #region Private functions

    private IEnumerator DelaySpawn(GameObject parentFolder, string nameEntityToSpawn, float delayBeforeSpawn)
    {
        m_isCurrentlySpawningEnemy = true;
        m_timeUntilSpawn = delayBeforeSpawn;
        m_timeUntilSpawnOriginal = delayBeforeSpawn;

        m_radialSpawnTimer.gameObject.SetActive(true);

        string enemyName = "Enemies/";

        if (m_spawnCustomEnemy && m_enemyToSpawn)
        {
            enemyName += m_enemyToSpawn.name;
        }
        else
        {
            enemyName += nameEntityToSpawn;
        }

        yield return new WaitUntil(() => m_timeUntilSpawn <= 0f); // Wait until timeUntilSpawn is 0 to spawn enemy

        GameObject entity = PhotonNetwork.InstantiateRoomObject(enemyName,
                                                      m_spawnPointTrigger.transform.position,
                                                      m_spawnPointTrigger.transform.rotation * Quaternion.Euler(0, 180f, 0),
                                                      0);
        m_spawnedEntity = entity;
        entity.transform.parent = parentFolder.transform; // set the entity as a child of the parentFolder, for organizational purposes

        m_isCurrentlySpawningEnemy = false;
    }

    #endregion

    #region Public functions

    // public wrapper for spawning the enemy, invoke the coroutine
    public void HandleSpawning(GameObject parentFolder, string nameEntityToSpawn, float delayBeforeSpawn)
    {
        StartCoroutine(DelaySpawn(parentFolder, nameEntityToSpawn, delayBeforeSpawn));
    }

    public bool IsSpawnSafe()
    {
        // simply returns true if the spawnedEntity got off the spawn, aka m_spawnedEntity is null
        return !m_spawnedEntity;
    }

    #endregion

    #region Photon functions

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Synchronize the time before spawning for each client.
        if (stream.IsWriting)
        {
            stream.SendNext(m_timeUntilSpawn);
        }
        else
        {
            m_timeUntilSpawn = (float)stream.ReceiveNext();
        }
    }

    #endregion
}
