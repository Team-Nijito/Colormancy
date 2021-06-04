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
    private float m_cumulativeMana = 0;

    #endregion

    #region Components

    private HealthScript m_health;

    // Player components
    private PlayerMovement m_playerMovement;
    private CharacterController m_controller;
    private PlayerAttack m_playerAttack;
    private ManaScript m_mana;

    // AI components
    private EnemyHitbox m_enemHitbox;
    private EnemyProjectileAbility m_enemProjectile;
    private EnemyMovement m_enemMovement;
    private EnemyTargeting m_enemTargetting;
    private NavMeshAgent m_enemNavMeshAgent;
    private Rigidbody m_enemRB;

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
            if (m_cumulativeDamage < 0)
            {
                photonView.RPC("Heal", RpcTarget.All, -m_cumulativeDamage);
            }
            if (m_cumulativeMana > 0)
            {
                photonView.RPC("RecoverMana", RpcTarget.All, m_cumulativeMana);
            }
        }
    }

    #endregion

    #region Private functions

    /// <summary>
    /// Loop through each status effect to see if the new status effect we're trying to add already exists
    /// If it does, then increment the time.
    /// </summary>
    /// <param name="type">Type of status effect we're trying to add</param>
    /// <param name="source">Source of status effect we're trying to add</param>
    /// <param name="duration">The increase in duration (if status effect already exists)</param>
    /// <returns>Does the status effect already exist in m_statusEffects?</returns>
    private bool CheckStatusEffectExist(StatusEffect.StatusType type, string source, float duration)
    {
        // First search the list to see if status effect exists
        foreach (StatusEffect effect in m_statusEffects)
        {
            if (effect.StatusEffectType == type)
            {
                if (source != null && effect.StatusSource != null)
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
        }
        return false;
    }

    /// <summary>
    /// (only one force status effect may exist in the list at one time, any new force names will essentially be ignored)
    /// </summary>
    /// <param name="duration">Additional duration of the force</param>
    /// <param name="dir">New force direction (to be combined with the existing force(s))</param>
    /// <param name="force">Magnitude for the force direction</param>
    /// <returns></returns>
    private bool CheckForceExist(float duration, Vector3 dir, float force)
    {
        // First search the list to see if status effect exists
        foreach (StatusEffect effect in m_statusEffects)
        {
            if (effect.StatusEffectType == StatusEffect.StatusType.Force)
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
        m_cumulativeMana = 0;

        // copying is inefficient, but is necessary in order to avoid the possibility
        // of the list being altered during the loop
        List<StatusEffect> copyList = new List<StatusEffect>(m_statusEffects);

        foreach (StatusEffect effect in copyList)
        {
            effect.Update();

            if (effect.StatusEffectType == StatusEffect.StatusType.DamageOverTime)
            {
                DamageOverTime DoT = (DamageOverTime)effect;
                m_cumulativeDamage += DoT.Damage;
            }
            if (effect.StatusEffectType == StatusEffect.StatusType.Rejuvenation)
            {
                Rejuvenation rej = (Rejuvenation)effect;
                m_cumulativeDamage -= rej.Damage;
                m_cumulativeMana += rej.Mana;
            }
            if (effect.StatusEffectType == StatusEffect.StatusType.ManaRegeneration)
            {
                ManaRegeneration manaReg = (ManaRegeneration)effect;
                m_cumulativeMana += manaReg.Mana * Time.deltaTime;
            }
        }
    }

    #endregion

    #region RPC Calls
    [PunRPC]
    private void ApplyStatus(StatusEffect.StatusType type, float duration, float secondsPerTick, float value, string source)
    {
        if (!CheckStatusEffectExist(type, source, duration))
        {
            StatusEffect newStatusEffect;

            switch (type)
            {
                case StatusEffect.StatusType.Blind:
                    if (m_isPlayer)
                        newStatusEffect = new Blind(m_statusEffects, type, duration, source, m_playerMovement.BlindPanel);
                    else
                        newStatusEffect = new Blind(m_statusEffects, type, duration, source, m_enemMovement, m_enemTargetting);

                    m_statusEffects.Add(newStatusEffect);
                    break;
                case StatusEffect.StatusType.DamageOverTime:
                    newStatusEffect = new DamageOverTime(m_statusEffects, type, duration, source, value, secondsPerTick);

                    m_statusEffects.Add(newStatusEffect);
                    break;
                case StatusEffect.StatusType.Slowdown:
                    if (m_isPlayer)
                        newStatusEffect = new Slow(m_statusEffects, type, duration, source, value, m_playerMovement);
                    else
                        newStatusEffect = new Slow(m_statusEffects, type, duration, source, value, m_enemMovement);

                    m_statusEffects.Add(newStatusEffect);
                    break;
                case StatusEffect.StatusType.Stun:
                    if (m_isPlayer)
                        newStatusEffect = new Stun(m_statusEffects, type, duration, source, m_playerMovement);
                    else
                        newStatusEffect = new Stun(m_statusEffects, type, duration, source, m_enemMovement);

                    m_statusEffects.Add(newStatusEffect);
                    break;
                case StatusEffect.StatusType.AutoAttackIncreasedDamage:
                    if (m_isPlayer)
                        throw new System.Exception("Cannot currently apply AutoAttackIncreasedDamage on players.");
                    else
                        newStatusEffect = new AutoAttackIncreasedDamage(m_statusEffects, type, duration, source, value, m_playerAttack);

                    m_statusEffects.Add(newStatusEffect);
                    break;
                case StatusEffect.StatusType.AttackLessDamage:
                    if (m_isPlayer)
                        throw new System.Exception("Cannot currently apply AttackLessDamage on players.");
                    else
                        newStatusEffect = new AttackLessDamage(m_statusEffects, type, duration, source, value, m_enemHitbox, m_enemProjectile);

                    m_statusEffects.Add(newStatusEffect);
                    break;
                case StatusEffect.StatusType.SpellIncreasedDamage:
                    if (m_isPlayer)
                        throw new System.Exception("Cannot currently apply SpellIncreasedDamage on players.");
                    else
                        newStatusEffect = new SpellIncreasedDamage(m_statusEffects, type, duration, source, value);

                    m_statusEffects.Add(newStatusEffect);
                    break;
                case StatusEffect.StatusType.AutoAttackIncreasedSpeed:
                    if (m_isPlayer)
                        newStatusEffect = new AutoAttackIncreasedSpeed(m_statusEffects, type, duration, source, value, m_playerAttack);
                    else
                        throw new System.Exception("Cannot currently apply AutoAttackIncreasedSpeed on enemies.");

                    m_statusEffects.Add(newStatusEffect);
                    break;
                case StatusEffect.StatusType.Rejuvenation:
                    if (m_isPlayer)
                        newStatusEffect = new Rejuvenation(m_statusEffects, type, duration, source, value, secondsPerTick, m_health, m_mana);
                    else
                        throw new System.Exception("Cannot currently apply Rejuvenation on enemies.");

                    m_statusEffects.Add(newStatusEffect);
                    break;
                case StatusEffect.StatusType.ManaRegeneration:
                    if (m_isPlayer)
                        newStatusEffect = new ManaRegeneration(m_statusEffects, type, duration, source, value, m_mana);
                    else
                        throw new System.Exception("Cannot currently apply ManaRegeneration on enemies.");

                    m_statusEffects.Add(newStatusEffect);
                    break;
            }
        }
    }

    /// <summary>
    /// (PunRPC) Apply a force over time effect if it doesn't already exist on this player, otherwise
    /// increase the duration of the effect.
    /// </summary>
    [PunRPC]
    private void ApplyForce(float duration, string source, Vector3 dir, float force)
    {
        if (!CheckForceExist(duration, dir, force))
        {
            if (m_isPlayer)
            {
                // Now make a new Force class and insert to StatusEffect list
                Force newForce = new Force(m_statusEffects, StatusEffect.StatusType.Force,
                                           duration, source, dir, force, m_controller);

                m_statusEffects.Add(newForce);
            }
            else
            {
                // Now make a new Force class and insert to StatusEffect list
                Force newForce = new Force(m_statusEffects, StatusEffect.StatusType.Force,
                                           duration, source, dir, force, m_enemMovement, gameObject.transform);

                m_statusEffects.Add(newForce);
            }
        }
    }

    [PunRPC]
    private void ClearAllStatusEffects()
    {
        foreach (StatusEffect s in m_statusEffects)
            s.Stop();

        m_statusEffects.Clear();
    }

    [PunRPC]
    private void ClearStatusEffect(StatusEffect.StatusType type, string source)
    {
        if (source != null)
        {
            StatusEffect statusEffect = m_statusEffects.Find(s => (s.GetStatusType() == type && s.StatusSource.Equals(source)));
            if (statusEffect != null)
            {
                statusEffect.Stop();
                m_statusEffects.Remove(statusEffect);
            }
        }
        else
        {
            StatusEffect statusEffect = m_statusEffects.Find(s => (s.GetStatusType() == type));
            if (statusEffect != null)
            {
                statusEffect.Stop();
                m_statusEffects.Remove(statusEffect);
            }
        }
    }

    #endregion

    #region Public functions
    /// <summary>
    /// Overarching function to apply statuses
    /// </summary>
    /// <param name="status">Status effect being applied.</param>
    /// <param name="duration">How long the status will last.</param>
    /// <param name="secondsPerTick">Only applies to fixed interval statuses such as DoT.</param>
    /// <param name="value">Value of the status being applied. Different depending on what the status is.</param>
    /// <param name="source">Source of the status.</param>
    public void RPCApplyStatus(StatusEffect.StatusType type, float duration = 0, float secondsPerTick = 0, float value = 0, string source = null)
    {
        photonView.RPC("ApplyStatus", m_isPlayer == true ? photonView.Owner : PhotonNetwork.MasterClient, type, duration, secondsPerTick, value, source);
    }

    /// <summary>
    /// Apply force to a character.
    /// </summary>
    /// <param name="dir">The direction of the force.</param>
    /// <param name="force">The magnitude of the force.</param>
    public void RPCApplyForce(float duration, string source, Vector3 dir, float force)
    {
        // disable stun, leave it to other class
        photonView.RPC("ApplyForce", m_isPlayer == true ? photonView.Owner : PhotonNetwork.MasterClient, duration, source, dir, force);
    }

    /// <summary>
    /// Clear all ongoing status effects.
    /// </summary>
    public void RPCClearAllStatusEffects()
    {
        photonView.RPC("ClearAllStatusEffects", m_isPlayer == true ? photonView.Owner : PhotonNetwork.MasterClient);
    }

    /// <summary>
    /// Clear a specific status effect.
    /// </summary>
    public void RPCClearStatusEffect(StatusEffect.StatusType type, string source = null)
    {
        photonView.RPC("ClearStatusEffect", m_isPlayer == true ? photonView.Owner : PhotonNetwork.MasterClient, type, source);
    }

    /// <summary>
    /// Separate function for finding existence of status
    /// </summary>
    public bool StatusExists(StatusEffect.StatusType type, string source = null)
    {
        if (source != null)
            return m_statusEffects.Find(s => (s.GetStatusType() == type && s.StatusSource.Equals(source))) != null;
        else
            return m_statusEffects.Find(s => (s.GetStatusType() == type)) != null;
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
                m_playerAttack = GetComponent<PlayerAttack>();
                m_mana = GetComponent<ManaScript>();
            }
            else
            {
                // This an AI entity
                m_enemHitbox = GetComponent<EnemyHitbox>();
                m_enemProjectile = GetComponent<EnemyProjectileAbility>();
                m_enemMovement = GetComponent<EnemyMovement>();
                m_enemTargetting = GetComponent<EnemyTargeting>();
                m_enemNavMeshAgent = GetComponent<NavMeshAgent>();
                m_enemRB = GetComponent<Rigidbody>();
            }

            m_statusEffects = new List<StatusEffect>();
        }
    }

    #endregion
}