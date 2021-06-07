using System.Collections.Generic;
using UnityEngine;

public class AmplifySpell : StatusEffect
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

    protected SpellManager m_spellManager;

    #endregion

    #region variables

    protected PlayerAttack m_playerAttack;
    public float m_increaseValue;

    #endregion

    #region Functions

    public AmplifySpell(List<StatusEffect> parentList, StatusType type, float duration, string source, float value, SpellManager spellManager)
                 : base(parentList, type, duration, source)
    {
        m_isPlayer = true;

        m_increaseValue = value;

        m_spellManager = spellManager;
        m_spellManager.AddDamageMultiplier(m_increaseValue);
    }

    /// <summary>
    /// Doesn't do anything.
    /// </summary>
    public override void DoStatusEffect() { }

    public override void Update() { }

    /// <summary>
    /// Remove this StatusEffect from a list of status effects, and reverts the blind.
    /// </summary>
    public override void Stop()
    {
        m_spellManager.AddDamageMultiplier(-m_increaseValue);

        base.Stop();
    }

    #endregion
}
