using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.AI;

public class HealthScript : MonoBehaviourPunCallbacks, IPunObservable
{
    // manages the "health" for any object
    // includes: damage, healing, armor? (damage reduction)
    // only works on gameobjects with a child canvas and UI slider

    public static GameObject LocalPlayerInstance;

    public bool m_isPlayer = false;

    public Slider m_healthBar;
    public Text m_username;

    [HideInInspector]
    public GameManager m_gameManager;

    [SerializeField]
    private float m_baseHealth = 100f;

    // used if you want to start off with baseHealth - initialHealthDeduction
    [SerializeField]
    private float m_initialHealthDeduction = 0f;

    // % of damage we're blocking
    [SerializeField]
    [Range(0f, 100f)]
    private float m_armorPercentage = 0f;

    [SerializeField]
    private bool m_isRegenHealth = false;

    [SerializeField]
    [Range(0f, 100f)]
    private float m_regenHealthPercentage = 1f;

    // health after buffs / whatever
    private float m_effectiveHealth;

    // max health after buffs / whatever
    private float m_maxEffectiveHealth;

    private ManaScript m_mScript;
    private AnimationManager m_animManager;
    private Transform m_healthBarTransform;

    [SerializeField]
    [Tooltip("Used for destroying dead enemies")]
    private float m_timeUntilDestroy = 3.0f;

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

    private void Awake()
    {
        // Try to find the GameManager script in the GameManager object in the current scene
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (m_isPlayer)
        {
            m_mScript = GetComponent<ManaScript>();

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
        m_effectiveHealth = m_baseHealth - m_initialHealthDeduction;
        m_maxEffectiveHealth = m_baseHealth;

        m_animManager = GetComponent<AnimationManager>();

        if (m_isPlayer)
        {
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
            if (m_effectiveHealth <= 0 && transform.gameObject.tag == "Player")
            {
                // player respawns in the middle
                photonView.RPC("RespawnPlayer", RpcTarget.All);
            }
        }
        else
        {
            if (m_effectiveHealth <= 0)
            {
                {
                    // disable movement, collider
                    GetComponent<EnemyChase>().enabled = false;
                    GetComponent<NavMeshAgent>().velocity = Vector3.zero;
                    GetComponent<NavMeshAgent>().enabled = false;
                    GetComponent<Collider>().enabled = false;

                    // disable health bar and name
                    m_healthBar.gameObject.SetActive(false);

                    // play dying animation
                    m_animManager.ChangeState(AnimationManager.EnemyState.Death);
                    // for other objects, we may want to destroy them
                    StartCoroutine(DelayedDestruction(m_timeUntilDestroy));
                }
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

    // Used for destroying dead enemies
    private IEnumerator DelayedDestruction(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        PhotonNetwork.Destroy(transform.gameObject);

        // Notify the Enemy Manager that an enemy has died
        PhotonView.Get(GameObject.Find("EnemyManager")).RPC("EnemyHasDied", RpcTarget.All);
        //GameObject.Find("EnemyManager").GetComponent<EnemyManager>().EnemyHasDied();
    }

    private void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (m_isPlayer)
        {
            // Associate the GameObject that this script belongs to with the player
            // so that if we ever invoke PhotonNetwork.PlayList
            // we can access a player's GameObject with: player.TagObject
            info.Sender.TagObject = gameObject;
        }
    }

    private void HealthBarFaceCamera()
    {
        // make the health bar orient towards the main camera
        if (m_healthBarTransform && Camera.main)
        {
            m_healthBarTransform.LookAt(Camera.main.transform);
        }
    }

    public float GetMaxEffectiveHealth() { return m_maxEffectiveHealth; }
    public float GetEffectiveHealth() { return m_effectiveHealth; }
    public float GetArmorPercentage() { return m_armorPercentage; }

    [PunRPC]
    public void AlterArmorValue(float armorPercent)
    {
        // replaces the armorPercentage with new value

        // ignore RPCs for dead enemies
        if (!m_isPlayer && m_animManager.GetCurrentState() == AnimationManager.EnemyState.Death)
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
    public void Heal(float healValue)
    {
        // heal formula
        // health = health + healValue
        // if health is larger than maxHealth, set health to maxHealth

        // ignore RPCs for dead enemies
        if (!m_isPlayer && m_animManager.GetCurrentState() == AnimationManager.EnemyState.Death)
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
        // damage formula
        // health = health - (damage - (damage * armorPercentage))
        
        // ignore RPCs for dead enemies
        if (!m_isPlayer && m_animManager.GetCurrentState() == AnimationManager.EnemyState.Death)
        {
            return;
        }

        if (damageValue <= 0)
            throw new ArgumentException(string.Format("{0} should be greater than zero", damageValue), "damageValue");
        if (photonView.IsMine)
        {
            m_effectiveHealth -= (damageValue - (m_armorPercentage / 100 * damageValue));
        }
    }

    [PunRPC]
    public void RespawnPlayer()
    {
        // if you want to teleport the player, just deactivate and reactivate the gameObject
        gameObject.SetActive(false);
        Quaternion spawnRotation = Quaternion.identity;
        transform.position = m_gameManager.ReturnSpawnpointPosition(ref spawnRotation);
        transform.rotation = spawnRotation;
        gameObject.SetActive(true);
        ResetHealth();
        m_mScript.ResetMana();
    }

    /// <summary>
    /// This should only be called when the player dies and respawns (caller should be a PunRPC function)
    /// </summary>
    public void ResetHealth()
    {
        m_effectiveHealth = m_maxEffectiveHealth;
    }
}
