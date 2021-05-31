using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MyBox;

public class SpawnpointBehaviour : MonoBehaviourPun
{
    #region Accessors (C# properties)

    // script for individual spawnpoints
    public bool IsSpawningEnemy { get { return m_isCurrentlySpawningEnemy; } private set { m_isCurrentlySpawningEnemy = value; } }

    #endregion

    #region Private fields

    [Separator("Custom spawn time for this spawnpoint")]
    [SerializeField]
    private bool m_hasUniqueSpawntime = false; // use this to indicate that we want to set the spawn time for this spawnpoint

    [ConditionalField(nameof(m_hasUniqueSpawntime))]
    [SerializeField]
    private float m_timeToSpawnEnemy = 3f;

    [Tooltip("Add a random delay to the spawn time, starting from 0 to the value in this property (inclusive)")]
    [SerializeField]
    private float m_addRandomDelay = 2f; // add a random delay to the spawnTime Range(0->m_addRandomDelay) inclusive

    [Separator("Custom enemies for this spawnpoint")]
    [SerializeField]
    private bool m_spawnCustomEnemy = false; // if true, select an enemy to spawn from the list of enemies in m_enemiesToSpawn

    [SerializeField]
    private GameObject[] m_enemiesToSpawn;

    [Separator("Misc. components")]
    [SerializeField]
    private GameObject m_spawnPointTrigger; // the trigger for the spawn, spawn the entity in the middle of this trigger 

    private GameObject m_spawnedEntity; 

    private float m_spawnpointRadius;

    private bool m_isCurrentlySpawningEnemy = false;

    // Spawn delay and GUI
    private float m_timeUntilSpawnOriginal = -1; 
    private float m_timeUntilSpawn = 0f; // count down to zero

    private float m_timeUntilUpdateDisplay = 0f; // we don't need to update every frame
    private const float TIME_UPDATE_DISPLAY = 0.2f; // every 0.2 seconds would suffice

    [SerializeField]
    private Image m_radialSpawnTimer;

    [SerializeField]
    private Transform m_radialPanel;

    [SerializeField]
    private Text m_displayText;

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
            if (!m_radialSpawnTimer.gameObject.activeInHierarchy)
            {
                m_radialPanel.gameObject.SetActive(true);
            }
            m_timeUntilSpawn -= Time.deltaTime;
            m_displayText.text = m_timeUntilSpawn.ToString("f1"); // keep only 1 decimal
            m_radialSpawnTimer.fillAmount = m_timeUntilSpawn / m_timeUntilSpawnOriginal;
            ProgressPanelFaceCamera();
        }
        else
        {
            if (m_radialSpawnTimer.gameObject.activeInHierarchy)
            {
                m_displayText.text = "";
                m_radialPanel.gameObject.SetActive(false);
            }
        }
    }

    #endregion

    #region Private functions

    private IEnumerator DelaySpawn(GameObject parentFolder, string nameEntityToSpawn, float delayBeforeSpawn)
    {
        m_isCurrentlySpawningEnemy = true;
        photonView.RPC("SyncTimeSpawn", RpcTarget.All, delayBeforeSpawn);
        PhotonNetwork.SendAllOutgoingCommands(); // send the RPC immediately

        m_timeUntilUpdateDisplay = 0f;

        string enemyName = "Enemies/";

        if (m_spawnCustomEnemy && m_enemiesToSpawn.Length > 0)
        {
            if (m_enemiesToSpawn.Length == 0)
            {
                enemyName += m_enemiesToSpawn[0].name; // choose the only enemy
            }
            else
            {
                enemyName += m_enemiesToSpawn[Random.Range(0, m_enemiesToSpawn.Length)].name; // choose a random enemy
            }
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

    // The progress gui faces the main camera (if it exists)
    private void ProgressPanelFaceCamera()
    {
        m_timeUntilUpdateDisplay += Time.deltaTime;

        // make the health bar orient towards the main camera
        if (m_timeUntilUpdateDisplay > TIME_UPDATE_DISPLAY && m_radialPanel && Camera.main)
        {
            m_timeUntilUpdateDisplay = 0f;
            m_radialPanel.rotation = Quaternion.LookRotation(m_radialPanel.position - Camera.main.transform.position);
        }
    }

    /// <summary>
    /// (PunRPC) Sync the timeUntilSpawn and timeUntilSpawnOriginal
    /// </summary>
    /// <param name="delayBeforeSpawn"></param>
    [PunRPC]
    private void SyncTimeSpawn(float delayBeforeSpawn)
    {
        m_timeUntilSpawn = delayBeforeSpawn;
        m_timeUntilSpawnOriginal = delayBeforeSpawn;
    }

    #endregion

    #region Public functions

    // public wrapper for spawning the enemy, invoke the coroutine
    public void HandleSpawning(GameObject parentFolder, string nameEntityToSpawn, float delayBeforeSpawn)
    {
        float delayTime = delayBeforeSpawn;

        if (m_hasUniqueSpawntime)
        {
            // use the m_timeToSpawnEnemy (this spawnpoint's property) instead of delayBeforeSpawn (which is given by the EnemyManager)
            delayTime = m_timeToSpawnEnemy;
        }

        delayTime += Random.Range(0, m_addRandomDelay); // add a random delay so that spawn times may be different
        StartCoroutine(DelaySpawn(parentFolder, nameEntityToSpawn, delayTime));
    }

    public bool IsSpawnSafe()
    {
        // simply returns true if the spawnedEntity got off the spawn, aka m_spawnedEntity is null
        return !m_spawnedEntity;
    }

    #endregion
}
