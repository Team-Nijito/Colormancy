using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyTargeting))]
[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(HealthScript))]
[RequireComponent(typeof(Rigidbody))]
[DisallowMultipleComponent]
public class StatusEffectScript : MonoBehaviourPun
{
    // Is responsible for handling side effects on entities with a HealthScript

    // TODO:
    // Damage over time (x)
    // Knock back (currently only for players | have to do AI next)
    // Slow
    // Stun
    // Fear / blind 
    // actually integrate all of these into one system

    #region Variables

    [System.Serializable]
    public struct DamageOverTime
    {
        public string name;
        public float dps;
        public float duration;
        public bool isPercentDamage; // is it 1 damage or 1% of health damage
    }

    // keep track of all our damage over time
    Dictionary<string, DamageOverTime> m_damageDict;

    [Range(0, 10f)]
    [SerializeField]
    private float m_statusEffectTickPerXSecond = 0.1f; // how often do you want to incur a status effect

    #endregion

    #region Components

    private HealthScript m_health;

    #endregion

    #region MonoBehaviour callbacks

    // Start is called before the first frame update
    void Start()
    {
        m_health = GetComponent<HealthScript>();

        m_damageDict = new Dictionary<string, DamageOverTime>();

        StartCoroutine(ApplyDamageEveryXSecond()); // run the loop every 0.1f second
    }

    #endregion

    #region Private functions

    // Used for applying damage
    private IEnumerator ApplyDamageEveryXSecond()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(m_statusEffectTickPerXSecond);

            if (m_damageDict.Count > 0)
            {
                // apply damage
                photonView.RPC("TakeDamage", RpcTarget.All, CalculateCumulativeDamage(m_statusEffectTickPerXSecond));
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
                returnDamage += (iter.Value.dps / 100) * m_health.MaxEffectiveHealth * XSecond;
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

    #endregion

    #region Public functions

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

    public void ClearDamageDict()
    {
        m_damageDict.Clear(); // reset all DoTs
    }

    public void RestartApplyDamage()
    {
        StartCoroutine(ApplyDamageEveryXSecond());
    }

    #endregion
}
