using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ManaScript : MonoBehaviour, IPunObservable
{
    // manages the mana for any player/object
    // includes: mana consumption, mana recovery, mana regen
    [SerializeField]
    private float m_baseMana = 100f;

    // used if you want to start off with baseMana - initialManaDeduction
    [SerializeField]
    private float m_initialManaDeduction = 0f;

    [SerializeField]
    private bool m_isRegenMana = false;

    [SerializeField]
    [Range(0f, 100f)]
    private float m_regenManaPercentage = 1f;

    // mana after buffs / whatever
    private float m_effectiveMana;

    // max mana after buffs / whatever
    private float m_maxEffectiveMana;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(m_effectiveMana);
        }
        else
        {
            m_effectiveMana = (float)stream.ReceiveNext();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_effectiveMana = m_baseMana - m_initialManaDeduction;
        m_maxEffectiveMana = m_baseMana;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isRegenMana)
        {
            ManaRegeneration(m_regenManaPercentage * Time.deltaTime);
        }
    }

    private void ManaRegeneration(float percentage)
    {
        if (percentage < 0)
            throw new ArgumentException(string.Format("{0} shouldn't be a negative number", percentage), "percentage");
        if (percentage > 100)
            throw new ArgumentException(string.Format("{0} should be less than or equal to 100", percentage), "percentage");
        float tempNewHealth = m_effectiveMana + m_maxEffectiveMana * (percentage / 100);
        if (tempNewHealth > m_maxEffectiveMana)
        {
            m_effectiveMana = m_maxEffectiveMana;
        }
        else
        {
            m_effectiveMana = tempNewHealth;
        }
    }

    public float GetMaxEffectiveMana() { return m_maxEffectiveMana; }
    public float GetEffectiveMana() { return m_effectiveMana; }

    [PunRPC]
    public void RecoverMana(float value)
    {
        // heal formula
        // health = health + healValue
        // if health is larger than maxHealth, set health to maxHealth
        if (value <= 0)
        {
            throw new ArgumentException(string.Format("{0} should be greater than zero", value), "value");
        }
        m_effectiveMana = m_effectiveMana > m_maxEffectiveMana ? m_maxEffectiveMana : m_effectiveMana + value;
    }

    [PunRPC]
    public void ConsumeMana(float value)
    {
        // damage formula
        // health = health - (damage - (damage * armorPercentage))
        if (value <= 0)
        {
            throw new ArgumentException(string.Format("{0} should be greater than zero", value), "value");
        }
        m_effectiveMana -= value;
    }

    /// <summary>
    /// This should only be called when the player dies and respawns (caller should be a PunRPC function)
    /// </summary>
    public void ResetMana()
    {
        m_effectiveMana = m_maxEffectiveMana;
    }
}
