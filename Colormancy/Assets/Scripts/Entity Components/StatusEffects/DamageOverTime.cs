using System.Collections.Generic;

public class DamageOverTime : StatusEffect
{
    /// <summary>
    /// Regardless of whether it's a player or AI entity, perform damage over time every tick for a specified duration.
    /// The damage can either be flat damage (example: 1 damage per second), or can be percentage based (example: 1% of hp damage per second)
    ///
    /// Multiple instances of this class can exist in the list of status effects at a time.
    /// (example of different DoTs: poison, bleed)
    /// </summary>

    #region Accessors (c# Properties)

    public float DamagePerSecond { get { return m_dps; } protected set { m_dps = value; } }
    public bool IsPercentDamage { get { return m_isPercentDamage; } protected set { m_isPercentDamage = value; } }
    
    #endregion

    #region Variables

    protected float m_dps;
    protected bool m_isPercentDamage; // is it 1 damage or 1% of health damage
    
    #endregion

    #region Functions

    public DamageOverTime(List<StatusEffect> parentList, string name, StatusType type, float duration, float DPS, bool isPercentDamage)
        : base(parentList, name, type, duration)
    {
        m_dps = DPS;
        m_isPercentDamage = isPercentDamage;
    }

    public override void DoStatusEffect()
    {
        // Actually don't do anything
        // I intend for a function in StatusEffectScript to calculate the cumulative damage per tick
        // (assuming that there are multiple DoT status effects on one entity)
        // and then to send that damage through the network.
        // I don't think it's necessary to send each damage source per tick through the network anyways
        // since the end result will basically be the same.
    }

    #endregion
}
