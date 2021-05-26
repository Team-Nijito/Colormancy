using System.Collections.Generic;
using UnityEngine;

public class SpellIncreasedDamage : StatusEffect
{
    /// <summary>
    /// The blind status effect blinds players by making a black screen appear and players cannot see their environment.
    /// 
    /// When any AI entities are blinded, they will not be able to see and target the player, and they will wander erratically 
    /// for the duration of the blind.
    /// 
    /// Only one instance of this class would exist in the list of status effects at a time.
    /// (you can either be blinded or not blinded, doesn't make sense for there to be different Blind status effects)
    /// </summary>

    #region Variables

    protected bool m_isPlayer;

    #endregion

    #region Player variables

    #endregion

    #region variables

    protected float m_increaseValue;

    #endregion

    #region Functions

    public SpellIncreasedDamage(List<StatusEffect> parentList, StatusType type, float duration, string source, float value)
                 : base(parentList, type, duration, source)
    {
        m_isPlayer = true;

        m_increaseValue = value;
    }

    /// <summary>
    /// Doesn't do anything.
    /// </summary>
    public override void DoStatusEffect() { }

    /// <summary>
    /// Remove this StatusEffect from a list of status effects, and reverts the blind.
    /// </summary>
    public override void Stop()
    {
        base.Stop();
    }

    #endregion
}
