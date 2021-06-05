using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class StatusEffect
{
    /// <summary>
    /// The generic StatusEffect class which cannot be instantiated.
    /// This is the parent class for the Blind, DamageOverTime, Force, Slowdown, and Stun classes.
    /// </summary>

    #region Accessors (c# Properties)

    // if you want to mutate the status effect values after class construction, use setters.
    public float Duration { get { return m_duration; } protected set { m_duration = value; } }
    public string StatusSource { get { return m_statusSource; } protected set { m_statusSource = value; } }
    public StatusType StatusEffectType { get { return m_statusType; } protected set { m_statusType = value; } }
    
    public List<StatusEffect> ParentList { get { return m_parentList; } protected set { m_parentList = value; } }

    #endregion

    #region Variables

    [System.Serializable]
    public enum StatusType
    {
        Blind,
        DamageOverTime,
        Force,
        Slowdown,
        Stun,
        AutoAttackIncreasedDamage,
        AttackLessDamage,
        SpellIncreasedDamage,
        AutoAttackIncreasedSpeed,
        Rejuvenation,
        ManaRegeneration,
        AutoAttackPoison,
        MovementIncreaseSpeed
    }

    protected List<StatusEffect> m_parentList; // the list the status effect is apart of, we need this reference so
                                               // we would be able to remove the StatusEffect from the list just by
                                               // invoking Tick like normal

    protected string m_statusSource;
    protected StatusType m_statusType;
    protected float m_duration;

    #endregion

    #region Public functions
    
    /// <summary>
    /// All status effects need to have these arguments, so the point is that their constructors would
    /// Invoke this constructor (to avoid repeating code setting the values).
    /// </summary>
    /// <param name="parentList">The list the status effect is apart of</param>
    /// <param name="name">The name of the status effect</param>
    /// <param name="type">The type of the status effect</param>
    /// <param name="duration">The duration of the status effect</param>
    protected StatusEffect(List<StatusEffect> parentList, StatusType type, float duration, string source)
    {
        m_parentList = parentList;

        m_statusSource = source;
        m_statusType = type;
        m_duration = duration;
    }

    /// <summary>
    /// Decrease the duration of the status effect.
    /// </summary>
    /// <param name="amount">Amount to decrease it by</param>
    public void DecreaseDuration(float amount)
    {
        m_duration -= amount;
    }

    /// <summary>
    /// Does whatever the status effect is supposed to do
    /// </summary>
    abstract public void DoStatusEffect();

    /// <summary>
    /// Increase the duration of the status effect.
    /// </summary>
    /// <param name="amount">Amount to increase it by</param>
    public void IncreaseDuration(float amount)
    {
        m_duration += amount;
    }

    /// <summary>
    /// Reset the duration of the status effect.
    /// </summary>
    /// <param name="amount">The duration to be set</param>
    public void SetDuration(float amount)
    {
        m_duration = amount;
    }

    /// <summary>
    /// Remove this StatusEffect from a list of status effects.
    /// </summary>
    public virtual void Stop()
    {
        if (m_parentList == null) return;

        m_parentList.Remove(this);
    }

    /// <summary>
    /// Decrease the duration by Time.deltaTime. If duration is zero, invoke Stop, otherwise invokes DoStatusEffect.
    /// </summary>
    public virtual void Update()
    {
        m_duration -= Time.deltaTime;
        if (m_duration <= 0)
        {
            Stop();
        }
        else
        {
            DoStatusEffect();
        }
    }

    public StatusType GetStatusType()
    {
        return m_statusType;
    }

    #endregion
}
