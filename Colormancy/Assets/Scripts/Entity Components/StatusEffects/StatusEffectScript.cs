using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StatusEffectScript : MonoBehaviourPun
{
    // Is responsible for handling side effects on entities with a HealthScript
    // Attach this component to entities that you want to be afflicted with status effects.
    // Features: Damage over Time, Knock back, Slow, Stun, and Blind
    // All statuses are implemented within their respective classes.

    #region Variables

    private List<StatusEffect> m_statusEffects;

    private bool m_isPlayer = false;

    private float m_cumulativeDamage = 0;

    #endregion

    #region Components

    private HealthScript m_health;

    // Player components
    private PlayerMovement m_playerMovement;
    private CharacterController m_controller;

    // AI components
    private EnemyMovement m_enemMovement;
    private EnemyTargeting m_enemTargetting;
    private NavMeshAgent m_enemNavMeshAgent;

    #endregion

    #region MonoBehaviour callbacks

    // Start is called before the first frame update
    void Start()
    {
        Initialization(true);
    }

    private void Update()
    {
        if (m_statusEffects.Count > 0)
        {
            LoopThroughStatusEffects();

            if (m_cumulativeDamage > 0)
            {
                // apply damage if there is any damage from status effects
                photonView.RPC("TakeDamage", RpcTarget.All, m_cumulativeDamage);
            }
        }
    }

    #endregion

    #region Private functions

    /// <summary>
    /// Loop through each status effect to see if the new status effect we're trying to add already exists
    /// If it does, then increment the time.
    /// </summary>
    /// <param name="name">Name of status effect we're trying to add</param>
    /// <param name="duration">The increase in duration (if status effect already exists)</param>
    /// <returns>Does the status effect already exist in m_statusEffects?</returns>
    private bool CheckStatusEffectExist(string name, float duration)
    {
        // First search the list to see if status effect exists
        foreach (StatusEffect effect in m_statusEffects)
        {
            if (effect.StatusName == name)
            {
                if (effect.StatusEffectType == StatusEffect.StatusType.Slowdown || effect.StatusEffectType == StatusEffect.StatusType.Stun)
                {
                    // reset duration, DO NOT STACK SLOW DURATION TIMES
                    effect.SetDuration(duration);
                }
                else
                {
                    effect.IncreaseDuration(duration);
                }
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// Overloaded variant of CheckStatusEffectExist intended for appending new forces to existing forces
    /// (given that they have the same name)
    /// </summary>
    /// <param name="name">Name of the force</param>
    /// <param name="duration">Additional duration of the force</param>
    /// <param name="dir">New force direction (to be combined with the existing force(s))</param>
    /// <param name="force">Magnitude for the force direction</param>
    /// <returns></returns>
    private bool CheckStatusEffectExist(string name, float duration, Vector3 dir, float force)
    {
        // First search the list to see if status effect exists
        foreach (StatusEffect effect in m_statusEffects)
        {
            if (effect.StatusName == name)
            {
                effect.IncreaseDuration(duration);
                ((Force)effect).AppendForce(dir, force);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Loop through each status effect this entity is currently afflicted with and acts them out.
    /// </summary>
    private void LoopThroughStatusEffects()
    {
        m_cumulativeDamage = 0;

        // copying is inefficient, but is necessary in order to avoid the possibility
        // of the list being altered during the loop
        List<StatusEffect> copyList = new List<StatusEffect>(m_statusEffects);

        foreach (StatusEffect effect in copyList)
        {
            effect.Tick();

            if (effect.StatusEffectType == StatusEffect.StatusType.DamageOverTime)
            {
                // if the StatusType is DamageOverTime, then
                // add this damage this DoT incurs per tick to the cumulative damage per tick
                DamageOverTime DoT = (DamageOverTime)effect;

                if (DoT.IsPercentDamage)
                {
                    m_cumulativeDamage += (DoT.DamagePerSecond / 100) * m_health.MaxEffectiveHealth * Time.deltaTime;
                }
                else
                {
                    m_cumulativeDamage += DoT.DamagePerSecond * Time.deltaTime;
                }
            }
        }
    }

    /// <summary>
    /// (PunRPC) Apply a stun effect if it doesn't already exist on this player, otherwise
    /// reset the duration of the stun effect (doesn't stack stun effects)
    /// </summary>
    [PunRPC]
    private void ApplyBlind(float duration)
    {
        if (!CheckStatusEffectExist("Blind", duration))
        {
            // Now make a new DamageOverTime class and insert to StatusEffect list
            if (m_isPlayer)
            {
                Blind newBlind = new Blind(m_statusEffects, "Blind", StatusEffect.StatusType.Stun,
                                       duration, m_playerMovement.BlindPanel);

                m_statusEffects.Add(newBlind);
            }
            else
            {
                Blind newBlind = new Blind(m_statusEffects, "Blind", StatusEffect.StatusType.Stun,
                                       duration, m_enemMovement, m_enemTargetting);

                m_statusEffects.Add(newBlind);
            }
        }
    }

    /// <summary>
    /// (PunRPC) Apply a force over time effect if it doesn't already exist on this player, otherwise
    /// increase the duration of the effect.
    /// </summary>
    [PunRPC]
    private void ApplyForceEffect(string name, float duration, Vector3 dir, float force, Vector3 enemPosition)
    {
        if (!CheckStatusEffectExist(name, duration, dir, force))
        {
            if (m_isPlayer)
            {
                // Now make a new Force class and insert to StatusEffect list
                Force newForce = new Force(m_statusEffects, name, StatusEffect.StatusType.Force,
                                           duration, dir, force, m_controller);

                m_statusEffects.Add(newForce);
            }
            else
            {
                // Now make a new Force class and insert to StatusEffect list
                Force newForce = new Force(m_statusEffects, name, StatusEffect.StatusType.Force,
                                           duration, dir, force, m_enemNavMeshAgent, enemPosition);

                m_statusEffects.Add(newForce);
            }
        }
    }

    /// <summary>
    /// (PunRPC) Apply a damage over time effect if it doesn't already exist on this player, otherwise
    /// increase the duration of the effect
    /// </summary>
    [PunRPC]
    private void ApplyOrStackDoTEffect(string name, float duration, float dmg, bool isPercentDmg)
    {
        if (!CheckStatusEffectExist(name, duration))
        {
            // Now make a new DamageOverTime class and insert to StatusEffect list
            DamageOverTime newDot = new DamageOverTime(m_statusEffects, name, StatusEffect.StatusType.DamageOverTime,
                                                       duration, dmg, isPercentDmg);

            m_statusEffects.Add(newDot);
        }
    }

    /// <summary>
    /// (PunRPC) Apply a slowdown effect if it doesn't already exist on this player, otherwise
    /// reset the duration of the slow effect with the same name (doesn't stack slow effects)
    /// </summary>
    [PunRPC]
    private void ApplySlowdown(string name, float percentReduction, float duration)
    {
        if (!CheckStatusEffectExist(name, duration))
        {
            // Now make a new DamageOverTime class and insert to StatusEffect list
            if (m_isPlayer)
            {
                Slow newSlow = new Slow(m_statusEffects, name, StatusEffect.StatusType.Slowdown,
                                       duration, percentReduction, m_playerMovement);

                m_statusEffects.Add(newSlow);
            }
            else
            {
                Slow newSlow = new Slow(m_statusEffects, name, StatusEffect.StatusType.Slowdown,
                                       duration, percentReduction, m_enemMovement);

                m_statusEffects.Add(newSlow);
            }
        }
    }

    /// <summary>
    /// (PunRPC) Apply a stun effect if it doesn't already exist on this player, otherwise
    /// reset the duration of the stun effect (doesn't stack stun effects)
    /// </summary>
    [PunRPC]
    private void ApplyStun(float duration)
    {
        if (!CheckStatusEffectExist("Stun", duration))
        {
            // Now make a new DamageOverTime class and insert to StatusEffect list
            if (m_isPlayer)
            {
                Stun newStun = new Stun(m_statusEffects, "Stun", StatusEffect.StatusType.Stun,
                                       duration, m_playerMovement);

                m_statusEffects.Add(newStun);
            }
            else
            {
                Stun newStun = new Stun(m_statusEffects, "Stun", StatusEffect.StatusType.Stun,
                                       duration, m_enemMovement);

                m_statusEffects.Add(newStun);
            }
        }
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
    public void RPCApplyForce(string name, float duration, Vector3 dir, float force, Vector3 originPosition)
    {
        // disable stun, leave it to other class

        if (m_isPlayer)
        {
            photonView.RPC("ApplyForceEffect", photonView.Owner, name, duration, dir, force, originPosition);
        }
        else
        {
            photonView.RPC("ApplyForceEffect", PhotonNetwork.MasterClient, name, duration, dir, force, originPosition);
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
            photonView.RPC("ApplyOrStackDoTEffect", photonView.Owner, name, duration, dmg, isPercentDmg);
        }
        else
        {
            photonView.RPC("ApplyOrStackDoTEffect", PhotonNetwork.MasterClient, name, duration, dmg, isPercentDmg);
        }
    }

    /// <summary>
    /// Apply a slowdown to this character for a duration, then changes the character's speed to its speed before the slowdown. Stackable.
    /// </summary>
    /// <param name="percentReduction">Range(0,100f). What percentage will we reduce the character's speed by? 50%?</param>
    /// <param name="duration">How long the slowdown will last.</param>
    public void RPCApplySlowdown(string name, float percentReduction, float duration)
    {
        if (m_isPlayer)
        {
            photonView.RPC("ApplySlowdown", photonView.Owner, name, percentReduction, duration);
        }
        else
        {
            photonView.RPC("ApplySlowdown", PhotonNetwork.MasterClient, name, percentReduction, duration);
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
    /// Clear all ongoing status effects.
    /// </summary>
    public void ClearStatusEffects()
    {
        m_statusEffects.Clear();
    }

    /// <summary>
    /// Fetch all references to other components. This is called once on Start() and more if we're missing any scene-dependent references whenever
    /// we switch to another scene
    /// </summary>
    public void Initialization(bool start=false)
    {
        if (start)
        {
            m_health = GetComponent<HealthScript>();

            if (m_controller = GetComponent<CharacterController>())
            {
                // This is a player entity
                m_isPlayer = true;
                m_playerMovement = GetComponent<PlayerMovement>();
            }
            else
            {
                // This an AI entity
                m_enemMovement = GetComponent<EnemyMovement>();
                m_enemTargetting = GetComponent<EnemyTargeting>();
                m_enemNavMeshAgent = GetComponent<NavMeshAgent>();
            }

            m_statusEffects = new List<StatusEffect>();
        }
    }

    #endregion
}