using System.Collections.Generic;
using UnityEngine;

public class Rejuvenation : StatusEffect
{
    /// <summary>
    /// Regardless of whether it's a player or AI entity, perform damage over time every tick for a specified duration.
    /// The damage can either be flat damage (example: 1 damage per second), or can be percentage based (example: 1% of hp damage per second)
    ///
    /// Multiple instances of this class can exist in the list of status effects at a time.
    /// (example of different DoTs: poison, bleed)
    /// </summary>

    #region Accessors (c# Properties)

    public float Damage { get { return m_currentDamage; } protected set { m_currentDamage = value; } }

    #endregion

    #region Variables

    protected float m_currentDamage;
    protected float m_damage;
    protected float m_secondsPerTick;

    protected float m_tickTime;

    #endregion

    #region Functions

    public Rejuvenation(List<StatusEffect> parentList, StatusType type, float duration, string source, float value, float secondsPerTick)
        : base(parentList, type, duration, source)
    {
        m_damage = value;
        m_secondsPerTick = secondsPerTick;
    }

    public override void DoStatusEffect()
    {
        // Actually don't do anything
        // I intend for a function in StatusEffectScript to calculate the cumulative damage per tick
        // (assuming that there are multiple DoT status effects on one entity)
        // and then to send that damage through the network.
        // I don't think it's necessary to send each damage source per tick through the network anyways
        // since the end result will basically be the same.

        // UPDATE: now works on a fixed interval set by the constructor.
        // The current damage is updated if the time goes over said interval.

        if (m_tickTime > m_secondsPerTick)
        {
            m_currentDamage = m_damage;
            m_tickTime -= m_secondsPerTick;
        }
        else
        {
            m_currentDamage = 0;
        }

        m_tickTime += Time.deltaTime;
    }

    #endregion
}
