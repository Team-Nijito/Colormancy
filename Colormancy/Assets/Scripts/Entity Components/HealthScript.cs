using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.AI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable; // to use with Photon's CustomProperties

[DisallowMultipleComponent]
public class HealthScript : MonoBehaviourPunCallbacks, IPunObservable
{
    // manages the "health" for any object, includes: damage, healing, armor? (damage reduction)
    // only works on gameobjects with a child canvas and UI slider

    #region Public variables and accessors (c# properties)

    public float MaxEffectiveHealth { get { return m_maxEffectiveHealth; } private set { m_maxEffectiveHealth = value; } }

    public static GameObject LocalPlayerInstance;

    public bool m_isPlayer = false;

    public Slider m_healthBar;
    public Text m_username;

    [HideInInspector]
    public GameManager m_gameManager;

    #endregion

    #region Private variables

    [SerializeField]
    private float m_baseHealth = 100f;

    // % of damage we're blocking
    [SerializeField]
    [Range(0f, 100f)]
    private float m_armorPercentage = 0f;

    [SerializeField]
    private bool m_isRegenHealth = false;

    [MyBox.ConditionalField("m_isRegenHealth", false)]
    [SerializeField]
    [Range(0f, 100f)]
    private float m_regenHealthPercentage = 1f;

    // health after buffs / whatever
    private float m_effectiveHealth;

    // max health after buffs / whatever
    private float m_maxEffectiveHealth;

    [SerializeField]
    [Tooltip("Used for destroying dead enemies")]
    private float m_timeUntilDestroy = 3.0f;

    private bool m_deathDebounce = false; // if we die, we don't want to invoke cleanup functions more than once!!

    [SerializeField]
    private bool m_playAnimationOnDeathIfAttacking = true; // only false if their attack animation is supposed to destroy the character (i.e. bomb)

    // spectating after death
    private GameObject m_spectateGhost;
    private GameObject m_mainCamera;

    #endregion

    #region Components

    // misc components
    private ManaScript m_mScript;
    private StatusEffectScript m_statusEffectScript;
    private EnemyAnimationManager m_animManager;
    private Chromaturgy.CameraController m_camController;
    private Transform m_healthBarTransform;

    #endregion

    #region MonoBehaviourCallback functions

    private void Awake()
    {
        // Try to find the GameManager script in the GameManager object in the current scene
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        m_isPlayer = GetComponent<CharacterController>() != null;
        if (m_isPlayer)
        {
            m_mScript = GetComponent<ManaScript>();
            m_camController = GetComponent<Chromaturgy.CameraController>();

            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;
            }

            if (!m_username)
            {
                Debug.LogError("Public Text variable m_text not set for HealthScript.cs! (it should be the text for the player's username)");
            }
            else
            {
                Player owner = photonView.Controller;
                m_username.text = owner.NickName;

                // Set the username as the object's name
                transform.name = m_username.text;
            }
            DontDestroyOnLoad(transform.root.gameObject); // made the warning go away with transform.root.gameObject
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_healthBarTransform = m_healthBar.transform;
        m_effectiveHealth = m_baseHealth;
        m_maxEffectiveHealth = m_baseHealth;

        m_statusEffectScript = GetComponent<StatusEffectScript>();
        m_animManager = GetComponent<EnemyAnimationManager>();

        if (m_isPlayer)
        {
            m_spectateGhost = Resources.Load<GameObject>("Player/SpectateGhost"); // load the ghost for players
            m_mainCamera = Resources.Load<GameObject>("Main Camera"); // load the main camera for reinstantiate for spectating

            // Associate the GameObject that this script belongs to with the player
            // so that if we ever invoke PhotonNetwork.PlayList
            // we can access a player's GameObject with: player.TagObject
            photonView.Owner.TagObject = gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isPlayer)
        {
            if (m_username.text == "Unnamed player")
            {
                // display usernames from players who are already in the room to new players who had just joined the room
                Player owner = photonView.Controller;
                m_username.text = owner.NickName;
            }
            // die
            if (m_effectiveHealth <= 0 && transform.gameObject.CompareTag("Player") && !m_deathDebounce)
            {
                m_deathDebounce = true;

                // inform the player's custom property that the player has died
                PhotonHashtable playerProperties = PhotonNetwork.LocalPlayer.CustomProperties;
                object isPlayerAliveProperty;
                if (playerProperties.TryGetValue(GameManager.PlayerAliveKey, out isPlayerAliveProperty))
                {
                    playerProperties[GameManager.PlayerAliveKey] = false;
                    PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
                }

                // player respawns as spectate camera
                photonView.RPC("RespawnPlayerAsGhost", PhotonNetwork.LocalPlayer);
                photonView.RPC("RemovePlayer", PhotonNetwork.LocalPlayer);
            }
        }
        else
        {
            // die
            if (m_effectiveHealth <= 0 && !m_deathDebounce)
            {
                m_deathDebounce = true;
                // tell everyone this enemy is deceased
                photonView.RPC("EnemyCleanup", RpcTarget.All);
            }
        }

        if (m_healthBar)
        {
            // slider goes from 0 to 100
            m_healthBar.value = (m_effectiveHealth / m_maxEffectiveHealth) * 100;
            HealthBarFaceCamera();
            if (m_isRegenHealth)
            {
                HealthRegeneration(m_regenHealthPercentage * Time.deltaTime);
            }
        }
    }

    #endregion

    #region Private functions

    // (PunRPC) Invoke to share the fact that this entity (at this point assumed to be an enemy) has died
    [PunRPC]
    private void EnemyCleanup()
    {
        EnemyChaserAI controllerScript = GetComponent<EnemyChaserAI>(); // works for all AI b/c all AI scripts derive from EnemyChaserAI
        controllerScript.StopAllTasks(); // stop all ongoing status effects, then disable any fellow scripts

        // disable health bar and name
        m_healthBar.gameObject.SetActive(false);

        // disable movement
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.velocity = Vector3.zero;
        agent.enabled = false;

        // make collider smaller so that we won't notice it when we bump into it
        // but also permit it to still be affected by any ongoing forces
        CapsuleCollider capCollider = GetComponent<CapsuleCollider>();
        capCollider.height = capCollider.radius;
        capCollider.center = new Vector3(0, 0.02f, 0);

        if (m_playAnimationOnDeathIfAttacking)
        {
            // play dying animation
            m_animManager.ChangeState(EnemyAnimationManager.EnemyState.Death);

            if (PhotonNetwork.IsMasterClient)
            {
                // only destroy this object if we're the master client
                StartCoroutine(DelayedDestruction(m_timeUntilDestroy));
            }
        }
        else
        {
            if (m_animManager.GetCurrentState() == EnemyAnimationManager.EnemyState.Attack)
            {
                // don't play dying animation, and simply destroy the object right away
                if (PhotonNetwork.IsMasterClient)
                {
                    // only destroy this object if we're the master client
                    StartCoroutine(DelayedDestruction(0));
                }
            }
            else
            {
                // play dying animation
                m_animManager.ChangeState(EnemyAnimationManager.EnemyState.Death);

                if (PhotonNetwork.IsMasterClient)
                {
                    // only destroy this object if we're the master client
                    StartCoroutine(DelayedDestruction(m_timeUntilDestroy));
                }
            }
        }
    }

    // Used for destroying dead enemies
    private IEnumerator DelayedDestruction(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        PhotonNetwork.Destroy(transform.gameObject);

        // Notify the Enemy Manager that an enemy has died
        GameObject manager = GameObject.Find("EnemyManager");
        if (manager)
            manager.GetComponent<EnemyManager>().EnemyHasDied();
    }

    // The healthbar gui faces the main camera (if it exists)
    private void HealthBarFaceCamera()
    {
        // make the health bar orient towards the main camera
        if (m_healthBarTransform && Camera.main)
        {
            m_healthBarTransform.LookAt(Camera.main.transform);
        }
    }

    /// <summary>
    /// This function is invoked during Update() to regen hp if m_isRegenHealth is enabled.
    /// </summary>
    /// <param name="percentage"></param>
    private void HealthRegeneration(float percentage)
    {
        if (percentage < 0)
            throw new ArgumentException(string.Format("{0} shouldn't be a negative number", percentage), "percentage");
        if (percentage > 100)
            throw new ArgumentException(string.Format("{0} should be less than or equal to 100", percentage), "percentage");
        float tempNewHealth = m_effectiveHealth + m_maxEffectiveHealth * (percentage / 100);
        if (tempNewHealth > m_maxEffectiveHealth)
            m_effectiveHealth = m_maxEffectiveHealth;
        else
            m_effectiveHealth = tempNewHealth;
    }

    #endregion

    #region Public functions

    public float GetMaxEffectiveHealth() { return m_maxEffectiveHealth; }
    public float GetEffectiveHealth() { return m_effectiveHealth; }
    public float GetArmorPercentage() { return m_armorPercentage; }

    [PunRPC]
    public void AlterArmorValue(float armorPercent)
    {
        // replaces the armorPercentage with new value

        // ignore RPCs for dead enemies
        if (!m_isPlayer && m_animManager.GetCurrentState() == EnemyAnimationManager.EnemyState.Death)
        {
            return;
        }

        if (armorPercent < 0)
            throw new ArgumentException(string.Format("{0} shouldn't be a negative number", armorPercent), "armorPercent");
        if (armorPercent > 100)
            throw new ArgumentException(string.Format("{0} should be less than or equal to 100", armorPercent), "armorPercent");
        m_armorPercentage = armorPercent;
    }

    [PunRPC]
    public void AlterArmorValueAdditive(float armorPercent, float resetTime = 0f)
    {
        // replaces the armorPercentage with new value

        // ignore RPCs for dead enemies
        if (m_animManager != null)
        {
            if (!m_isPlayer && m_animManager.GetCurrentState() == EnemyAnimationManager.EnemyState.Death)
            {
                return;
            }
        }
        else
        {
            if (!m_isPlayer)
            {
                return;
            }
        }

        m_armorPercentage += armorPercent;
        m_armorPercentage = Mathf.Clamp(m_armorPercentage, 0f, 100f);

        if (resetTime != 0f)
        {
            IEnumerator resetCoroutine = ResetArmor(armorPercent, resetTime);
            StartCoroutine(resetCoroutine);
        }
    }

    IEnumerator ResetArmor(float armorPercent, float resetTime)
    {
        yield return new WaitForSeconds(resetTime);
        m_armorPercentage -= armorPercent;
    }

    [PunRPC]
    public void Heal(float healValue)
    {
        // heal formula: health = health + healValue
        // if health is larger than maxHealth, set health to maxHealth

        // ignore RPCs for dead enemies
        if (!m_isPlayer && m_animManager.GetCurrentState() == EnemyAnimationManager.EnemyState.Death)
        {
            return;
        }

        if (healValue <= 0)
            throw new ArgumentException(string.Format("{0} should be greater than zero", healValue), "healValue");
        m_effectiveHealth = m_effectiveHealth > m_maxEffectiveHealth ? m_maxEffectiveHealth : m_effectiveHealth + healValue;
    }

    [PunRPC]
    public void TakeDamage(float damageValue)
    {
        // damage formula: health = health - (damage - (damage * armorPercentage))
        //Debug.Log(("Taking: ", damageValue, " damage"));

        // ignore RPCs for dead enemies
        if (m_animManager != null)
        {
            if (!m_isPlayer && m_animManager.GetCurrentState() == EnemyAnimationManager.EnemyState.Death)
            {
                return;
            }
        }

        if (damageValue <= 0)
            throw new ArgumentException(string.Format("{0} should be greater than zero", damageValue), "damageValue");
        if (photonView.IsMine)
        {
            m_effectiveHealth -= (damageValue - (m_armorPercentage / 100 * damageValue));
        }
    }

    /// <summary>
    /// (PunRPC) Send this RPC call to the ownwer of the player if you want to destroy this particular player gameObject.
    /// </summary>
    [PunRPC]
    public void RemovePlayer()
    {
        if (gameObject && photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    /// <summary>
    /// (PunRPC) Respawns a player at the spawnpoint they've spawned in, and reset all necessary private fields in any player-related
    /// components that are initialized at start / awake.
    /// </summary>
    [PunRPC]
    public void RespawnPlayer()
    {
        // if you want to teleport the player, just deactivate and reactivate the gameObject
        gameObject.SetActive(false);

        // if we have a status effect component, then stop any onging status effects
        if (m_statusEffectScript)
        {
            m_statusEffectScript.RPCClearAllStatusEffects();
        }

        if (!m_gameManager)
        {
            // Must find the gameManager because we loaded into the new scene
            m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }

        Quaternion spawnRotation = Quaternion.identity;
        transform.position = m_gameManager.ReturnSpawnpointPosition(ref spawnRotation);
        transform.rotation = spawnRotation;
        m_camController.ResetRotation(spawnRotation);

        ResetHealth();
        m_mScript.ResetMana();
        gameObject.SetActive(true);
    }

    /// <summary>
    /// (PunRPC) Players will spawn in as a ghost. The player who died should send this RPC call to themself.
    /// </summary>
    [PunRPC]
    public void RespawnPlayerAsGhost(PhotonMessageInfo info)
    {
        // deactivate the gameObject for now
        gameObject.SetActive(false);

        // spawn spectate ghost
        Quaternion spawnRotation = Quaternion.identity;
        Vector3 spawnPosition = m_gameManager.ReturnSpawnpointPosition(ref spawnRotation);
        GameObject spectator = PhotonNetwork.Instantiate("Player/" + m_spectateGhost.name, spawnPosition, spawnRotation);
        spectator.GetComponent<Chromaturgy.CameraController>().SetIsSpectateCamera(true);

        // Instantiate a camera
        Instantiate(m_mainCamera);

        // Disable UI stuff (both OrbTray and the OrbUI to indicate that you can't cast spells)
        GameObject canvas = GameObject.Find("Canvas");
        canvas.transform.Find("LevelUI").transform.Find("OrbTray").gameObject.SetActive(false);
        canvas.transform.Find("OrbUI(Clone)").gameObject.SetActive(false);
    }

    /// <summary>
    /// This should only be called when the player dies and respawns (caller should be a PunRPC function)
    /// </summary>
    public void ResetHealth()
    {
        m_effectiveHealth = m_maxEffectiveHealth;
        m_deathDebounce = false;
    }

    /// <summary>
    /// Sets the effective health to 0, killing the entity.
    /// </summary>
    public void ZeroHealth()
    {
        m_effectiveHealth = 0;
    }

    #endregion

    #region Photon functions

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(m_effectiveHealth);
        }
        else
        {
            m_effectiveHealth = (float)stream.ReceiveNext();
        }
    }

    #endregion
}
