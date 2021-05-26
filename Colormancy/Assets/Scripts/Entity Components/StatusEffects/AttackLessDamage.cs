using System.Collections.Generic;
using UnityEngine;

public class AttackLessDamage : StatusEffect
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

    protected EnemyHitbox m_enemHitbox;
    protected EnemyProjectileAbility m_enemProjectile;
    protected float m_value;

    #endregion

    #region Functions

    public AttackLessDamage(List<StatusEffect> parentList, StatusType type, float duration, string source, float value, EnemyHitbox enemHitbox, EnemyProjectileAbility enemProjectile)
                 : base(parentList, type, duration, source)
    {
        m_isPlayer = false;

        m_value = value;
        m_enemHitbox = enemHitbox;
        m_enemProjectile = enemProjectile;

        if (m_enemHitbox)
        {
            foreach (EnemyHitbox.HitBox h in m_enemHitbox.HitBoxesArray)
                h.m_hitBoxScript.AddDamageMultiplier(m_value);
        }

        if (m_enemProjectile)
            m_enemProjectile.RPCAddDamageMultiplier(m_value);
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
        if (m_enemHitbox)
        {
            foreach (EnemyHitbox.HitBox h in m_enemHitbox.HitBoxesArray)
                h.m_hitBoxScript.AddDamageMultiplier(-m_value);
        }

        if (m_enemProjectile)
            m_enemProjectile.RPCAddDamageMultiplier(-m_value);

        base.Stop();
    }

    #endregion
}
