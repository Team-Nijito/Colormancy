using System.Collections.Generic;
using UnityEngine;

public class Stun : StatusEffect
{
    /// <summary>
    /// The stun status effect will prevent players from moving their characters, 
    /// can cast spells (also probably a bug)
    /// 
    /// When any AI entities are stunned, their momentum will stop and they won't do anything.
    /// 
    /// Only one instance of this class would exist in the list of status effects at a time.
    /// (you can either be stunned or not stunned, doesn't make sense for there to be different Stun status effects)
    /// </summary>

    #region Variables

    protected bool m_isPlayer;

    #endregion

    #region Player variables

    protected PlayerMovement m_playMove;

    #endregion

    #region AI variables

    protected EnemyMovement m_enemyMove;

    #endregion

    #region Functions

    public Stun(List<StatusEffect> parentList, StatusType type, float duration, string source, PlayerMovement pMove)
                 : base(parentList, type, duration, source)
    {
        m_isPlayer = true;
        m_playMove = pMove;

        m_playMove.Stun();
    }

    public Stun(List<StatusEffect> parentList, StatusType type, float duration, string source, EnemyMovement eMove)
                 : base(parentList, type, duration, source)
    {
        m_isPlayer = false;
        m_enemyMove = eMove;

        m_enemyMove.DisableAgent(true, true);
    }

    /// <summary>
    /// Does nothing. The effect is already applied during the constructor so just do nothing.
    /// </summary>
    public override void DoStatusEffect()
    {
        return;
    }

    /// <summary>
    /// Remove this StatusEffect from a list of status effects, and reverts the stun.
    /// </summary>
    public override void Stop()
    {
        // Revert slowdown
        if (m_isPlayer)
        {
            m_playMove.UnStun();
        }
        else
        {
            m_enemyMove.EnableAgent();
        }

        base.Stop();
    }

    /// <summary>
    /// Decrease the duration by Time.deltaTime. If duration is zero, invoke Stop.
    /// </summary>
    public override void Update()
    {
        m_duration -= Time.deltaTime;
        if (m_duration <= 0)
        {
            Stop();
        }
    }

    #endregion
}
