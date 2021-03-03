using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.AI;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class HealthScript : MonoBehaviourPunCallbacks, IPunObservable
{
    // manages the "health" for any object
    // includes: damage, healing, armor? (damage reduction)
    // only works on gameobjects with a child canvas and UI slider

    #region Public variables

    // DOT occurs every second
    [System.Serializable]
    public struct DamageOverTime 
    {
        public string name;
        public float dps;
        public float duration;
        public bool isPercentDamage; // is it 1 damage or 1% of health damage
    }

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

    // used if you want to start off with baseHealth - initialHealthDeduction
    // for debugging
    //[SerializeField]
    //private float m_initialHealthDeduction = 0f;

    // % of damage we're blocking
    [SerializeField]
    [Range(0f, 100f)]
    private float m_armorPercentage = 0f;

    [SerializeField]
    private bool m_isRegenHealth = false;

    [SerializeField]
    [Range(0f, 100f)]
    private float m_regenHealthPercentage = 1f;

    [SerializeField]
    private float m_damageEveryXSecond = 0.1f; // when do you want to incur DoT

    // health after buffs / whatever
    private float m_effectiveHealth;

    // max health after buffs / whatever
    private float m_maxEffectiveHealth;

    [SerializeField]
    [Tooltip("Used for destroying dead enemies")]
    private float m_timeUntilDestroy = 3.0f;

    // keep track of all our damage over time
    Dictionary<string, DamageOverTime> m_damageDict;

    // misc components
    private ManaScript m_mScript;
    private EnemyAnimationManager m_animManager;
    private Chromaturgy.CameraController m_camController;
    private Transform m_healthBarTransform;

    #endregion
    #region MonoBehaviourCallback functions

    private void Awake()
    {
        // Try to find the GameManager script in the GameManager object in the current scene
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
        m_effectiveHealth = m_baseHealth; // - m_initialHealthDeduction;
        m_maxEffectiveHealth = m_baseHealth;

        m_damageDict = new Dictionary<string, DamageOverTime>();

        m_animManager = GetComponent<EnemyAnimationManager>();

        if (m_isPlayer)
        {
            // Associate the GameObject that this script belongs to with the player
            // so that if we ever invoke PhotonNetwork.PlayList
            // we can access a player's GameObject with: player.TagObject
            photonView.Owner.TagObject = gameObject;
        }

        StartCoroutine(ApplyDamageEveryXSecond(m_damageEveryXSecond)); // run the loop every 0.1f second
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
                    GetComponent<EnemyChaserAI>().enabled = false; // works for all AI b/c all AI scripts derive from EnemyChaserAI
                    GetComponent<NavMeshAgent>().velocity = Vector3.zero;
                    GetComponent<NavMeshAgent>().enabled = false;
                    GetComponent<Collider>().enabled = false;

                    // disable health bar and name
                    m_healthBar.gameObject.SetActive(false);

                    // play dying animation
                    m_animManager.ChangeState(EnemyAnimationManager.EnemyState.Death);
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

    #endregion
    #region Private functions

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
    }

    // Used for applying damage
    private IEnumerator ApplyDamageEveryXSecond(float XSecond)
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(XSecond);

            if (m_damageDict.Count > 0)
            {
                // apply damage
                photonView.RPC("TakeDamage", RpcTarget.All, CalculateCumulativeDamage(XSecond));
            }
        }
    }

    // Go over every DamageOverTime in m_damageDict and apply the damage and decrease each
    // struct's duration
    // this function is semi flawed ... what if the duration of the DoT is 0.7f
    // and we do damage every 1 second? (temp solution is to call this funct with small XSecond values (<1))
    private float CalculateCumulativeDamage(float XSecond)
    {
        float returnDamage = 0;

        // copying is inefficient, but is necessary in order to avoid the possibility
        // of the dictionary being altered during the loop
        Dictionary<string, DamageOverTime> copyDict = new Dictionary<string, DamageOverTime>(m_damageDict);
        List<string> keysToDelete = new List<string>();

        foreach (KeyValuePair<string, DamageOverTime> iter in copyDict)
        {
            print("Taking " + iter.Key + "damage");
            if (iter.Value.isPercentDamage)
            {
                returnDamage += (iter.Value.dps / 100) * m_maxEffectiveHealth * XSecond;
            }
            else
            {
                returnDamage += iter.Value.dps * XSecond;
            }
            
            DamageOverTime newStruct = iter.Value;
            newStruct.duration -= XSecond;

            if (newStruct.duration <= 0)
            {
                keysToDelete.Add(iter.Key);
            }
            else
            {
                m_damageDict[iter.Key] = newStruct;
            }
        }

        // delete any marked keys in m_damageDict
        foreach (string key in keysToDelete)
        {
            m_damageDict.Remove(key);
        }

        return returnDamage;
    }

    private void HealthBarFaceCamera()
    {
        // make the health bar orient towards the main camera
        if (m_healthBarTransform && Camera.main)
        {
            m_healthBarTransform.LookAt(Camera.main.transform);
        }
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
    public void Heal(float healValue)
    {
        // heal formula
        // health = health + healValue
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
        // damage formula
        // health = health - (damage - (damage * armorPercentage))
        
        // ignore RPCs for dead enemies
        if (!m_isPlayer && m_animManager.GetCurrentState() == EnemyAnimationManager.EnemyState.Death)
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
        m_camController.ResetRotation(spawnRotation);
        gameObject.SetActive(true);
        ResetHealth();
        m_damageDict.Clear(); // reset all DoTs
        StartCoroutine(ApplyDamageEveryXSecond(m_damageEveryXSecond)); // restart Coroutine
        m_mScript.ResetMana();
    }

    /// <summary>
    /// (PunRPC) Apply a damage over time effect if it doesn't already exist on this player, otherwise
    /// increase the duration of the effect
    /// </summary>
    [PunRPC]
    public void ApplyOrStackDoT(bool isPercentDmg, float dmg, float duration, string name)
    {
        DamageOverTime newDoT;
        newDoT.name = name;
        newDoT.dps = dmg;
        newDoT.duration = duration;
        newDoT.isPercentDamage = isPercentDmg;

        if (m_damageDict.ContainsKey(name))
        {
            float oldDuration = m_damageDict[name].duration;
            newDoT.duration += oldDuration;
        }
        m_damageDict[name] = newDoT;
    }

    /// <summary>
    /// This should only be called when the player dies and respawns (caller should be a PunRPC function)
    /// </summary>
    public void ResetHealth()
    {
        m_effectiveHealth = m_maxEffectiveHealth;
    }

    #endregion
    #region Photon functions

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
