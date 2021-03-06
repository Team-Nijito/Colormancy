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
    // Features: Damage over Time, Knock back, Slow, Stun, and Blind
    // Damage over Time is implemented in this script, while
    // all other status effects are implemented in the controller script that implements
    // IStatusEffect

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
    private Dictionary<string, DamageOverTime> m_damageDict;

    [Range(0, 10f)]
    [SerializeField]
    private float m_statusEffectTickPerXSecond = 0.1f; // how often do you want to incur a status effect

    private bool m_isPlayer = false;

    #endregion

    #region Components

    private HealthScript m_health;
    private IStatusEffects m_controllerScript;

    #endregion

    #region MonoBehaviour callbacks

    // Start is called before the first frame update
    void Start()
    {
        m_health = GetComponent<HealthScript>();

        if (GetComponent<CharacterController>())
        {
            m_controllerScript = GetComponent<PlayerMovement>(); // This is a player
            m_isPlayer = true;
        }
        else
        {
            m_controllerScript = GetComponent<EnemyChaserAI>(); // This an AI
        }

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
            //print("Taking " + iter.Key + "damage");
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

    /// <summary>
    /// (PunRPC) Apply a damage over time effect if it doesn't already exist on this player, otherwise
    /// increase the duration of the effect
    /// </summary>
    [PunRPC]
    private void ApplyOrStackDoT(bool isPercentDmg, float dmg, float duration, string name)
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

    #endregion

    #region Public functions
    
    /// <summary>
    /// Blind a characters. If it's an AI, they will panic and will not be able to target players.
    /// </summary>
    /// <param name="duration">How long the stun will last.</param>
    public void RPCApplyBlind(float duration)
    {
        if (m_isPlayer)
        {
            photonView.RPC("ApplyBlind", photonView.Owner, duration);
        }
        else
        {
            photonView.RPC("ApplyBlind", PhotonNetwork.MasterClient, duration);
        }
    }

    /// <summary>
    /// Apply force to a character.
    /// </summary>
    /// <param name="dir">The direction of the force.</param>
    /// <param name="force">The magnitude of the force.</param>
    public void RPCApplyForce(Vector3 dir, float force)
    {
        if (m_isPlayer)
        {
            photonView.RPC("ApplyForce", photonView.Owner, dir, force);
        }
        else
        {
            photonView.RPC("ApplyForce", PhotonNetwork.MasterClient, dir, force);
        }
    }

    /// <summary>
    /// Apply or stack damage over time to a character.
    /// </summary>
    /// <param name="isPercentDmg">Is a damage is percent value instead of a flat number?</param>
    /// <param name="dmg">Damage done every second.</param>
    /// <param name="duration">How long the damage over time lasts</param>
    /// <param name="name">Name of this damage over time </param>
    public void RPCApplyOrStackDoT(bool isPercentDmg, float dmg, float duration, string name)
    {
        if (m_isPlayer)
        {
            photonView.RPC("ApplyOrStackDoT", photonView.Owner, isPercentDmg, dmg, duration, name);
        }
        else
        {
            photonView.RPC("ApplyOrStackDoT", PhotonNetwork.MasterClient, isPercentDmg, dmg, duration, name);
        }
    }

    /// <summary>
    /// Apply a slowdown to this character for a duration, then changes the character's speed to its speed before the slowdown. Stackable.
    /// </summary>
    /// <param name="percentReduction">Range(0,100f). What percentage will we reduce the character's speed by? 50%?</param>
    /// <param name="duration">How long the slowdown will last.</param>
    public void RPCApplySlowdown(float percentReduction, float duration)
    {
        if (m_isPlayer)
        {
            photonView.RPC("ApplySlowdown", photonView.Owner, percentReduction, duration);
        }
        else
        {
            photonView.RPC("ApplySlowdown", PhotonNetwork.MasterClient, percentReduction, duration);
        }
    }

    /// <summary>
    /// Stuns a character and prevent them from moving.
    /// </summary>
    /// <param name="duration">How long the stun will last.</param>
    public void RPCApplyStun(float duration)
    {
        if (m_isPlayer)
        {
            photonView.RPC("ApplyStun", photonView.Owner, duration);
        }
        else
        {
            photonView.RPC("ApplyStun", PhotonNetwork.MasterClient, duration);
        }
    }

    /// <summary>
    /// Clear all ongoing damage over time status effects.
    /// </summary>
    public void ClearDamageDict()
    {
        m_damageDict.Clear(); // reset all DoTs
    }

    /// <summary>
    /// Restarts the damage every x second coroutine, this is usually called after a player dies.
    /// </summary>
    public void RestartApplyDamage()
    {
        StartCoroutine(ApplyDamageEveryXSecond());
    }

    #endregion
}
