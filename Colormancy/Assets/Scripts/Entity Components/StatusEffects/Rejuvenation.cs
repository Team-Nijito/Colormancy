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
    public float Mana { get { return m_currentMana; } protected set { m_currentMana = value; } }

    #endregion

    #region Variables

    protected float m_currentDamage;
    protected float m_currentMana;
    protected float m_damage;
    protected float m_mana;
    protected float m_secondsPerTick;

    protected float m_tickTime;

    #endregion

    #region Functions

    public Rejuvenation(List<StatusEffect> parentList, StatusType type, float duration, string source, float value, float secondsPerTick, HealthScript health, ManaScript mana)
        : base(parentList, type, duration, source)
    {
        m_damage = health.GetMaxEffectiveHealth() * value / 100;
        m_mana = mana.GetMaxEffectiveMana() * value / 100;
        m_secondsPerTick = secondsPerTick;
    }

    public override void DoStatusEffect()
    {
        if (m_tickTime > m_secondsPerTick)
        {
            m_currentDamage = m_damage;
            m_currentMana = m_mana;
            m_tickTime -= m_secondsPerTick;
        }
        else
        {
            m_currentDamage = 0;
            m_currentMana = 0;
        }

        m_tickTime += Time.deltaTime;
    }

    #endregion
}
