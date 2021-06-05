using System.Collections.Generic;
using UnityEngine;

public class ManaRegeneration : StatusEffect
{
    /// <summary>
    /// Regardless of whether it's a player or AI entity, perform damage over time every tick for a specified duration.
    /// The damage can either be flat damage (example: 1 damage per second), or can be percentage based (example: 1% of hp damage per second)
    ///
    /// Multiple instances of this class can exist in the list of status effects at a time.
    /// (example of different DoTs: poison, bleed)
    /// </summary>

    #region Accessors (c# Properties)
    public float Mana { get { return m_currentMana; } protected set { m_currentMana = value; } }

    #endregion

    #region Variables

    protected ManaScript m_manaScript;
    protected float m_manaRegenPercent;
    protected float m_extraPercent;
    protected float m_maxMana;
    protected float m_currentMana;

    #endregion

    #region Functions

    public ManaRegeneration(List<StatusEffect> parentList, StatusType type, float duration, string source, float value, ManaScript mana)
        : base(parentList, type, duration, source)
    {
        m_manaScript = mana;

        m_extraPercent = value;
        m_manaRegenPercent = m_manaScript.GetManaRegen();
        m_maxMana = m_manaScript.GetMaxEffectiveMana();

        m_currentMana = (m_extraPercent / 100) * m_maxMana * (m_manaRegenPercent / 100);
    }

    public override void DoStatusEffect()
    {

    }

    public override void Update()
    {
        
    }

    #endregion
}
