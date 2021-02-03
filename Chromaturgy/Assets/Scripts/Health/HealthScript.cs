using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class HealthScript : MonoBehaviourPunCallbacks, IPunObservable
{
    // manages the "health" for any object
    // includes: damage, healing, armor? (damage reduction)
    // only works on gameobjects with a child canvas and UI slider

    public static GameObject LocalPlayerInstance;
    public Slider m_healthBar;
    public Text m_username;

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

    private Transform m_healthBarTransform;

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
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_healthBarTransform = m_healthBar.transform;
        m_effectiveHealth = m_baseHealth - m_initialHealthDeduction;
        m_maxEffectiveHealth = m_baseHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_username.text == "Unnamed player")
        {
            // display usernames from players who are already in the room to new players who had just joined the room
            Player owner = photonView.Controller;
            m_username.text = owner.NickName;
        }


        if (m_effectiveHealth <= 0)
        {
            // die
            if (transform.gameObject.tag == "Player")
            {
                // TODO: actual death + respawn system

                // for player objects, just make them inactive
                // for players who are killed, the scene won't be updated anymore (looks like player crashed)
                //transform.gameObject.SetActive(false);

                // bootleg respawn which doesn't work b/c we're syncing transforms and health
                //transform.position = new Vector3(0, 5, 0);
                //m_effectiveHealth = m_maxEffectiveHealth;
            }
            else
            {
                // for other objects, we may want to destroy them
                Destroy(transform.gameObject);
            }
        }
        // slider goes from 0 to 100
        m_healthBar.value = (m_effectiveHealth / m_maxEffectiveHealth) * 100;
        HealthBarFaceCamera();
        if (m_isRegenHealth) 
            HealthRegeneration(m_regenHealthPercentage * Time.deltaTime);
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

    public float GetMaxEffectiveHealth() { return m_maxEffectiveHealth; }
    public float GetEffectiveHealth() { return m_effectiveHealth; }
    public float GetArmorPercentage() { return m_armorPercentage; }

    private void HealthBarFaceCamera()
    {
        // make the health bar orient towards the main camera
        m_healthBarTransform.LookAt(Camera.main.transform);
    }

    [PunRPC]
    public void AlterArmorValue(float armorPercent)
    {
        // replaces the armorPercentage with new value
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
        if (healValue <= 0)
            throw new ArgumentException(string.Format("{0} should be greater than zero", healValue), "healValue");
        m_effectiveHealth = m_effectiveHealth > m_maxEffectiveHealth ? m_maxEffectiveHealth : m_effectiveHealth + healValue;
    }

    [PunRPC]
    public void TakeDamage(float damageValue)
    {
        // damage formula
        // health = health - (damage - (damage * armorPercentage))
        if (damageValue <= 0)
            throw new ArgumentException(string.Format("{0} should be greater than zero", damageValue), "damageValue");
        m_effectiveHealth -= (damageValue - (m_armorPercentage / 100 * damageValue));
    }
}
