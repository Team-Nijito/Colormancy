using System.Collections.Generic;
using UnityEngine;

public class Slow : StatusEffect
{
    /// <summary>
    /// The slow status effects make entities slow. This is done by reducing their speed percentage wise (% speed deduction, example 30% speed deduction).
    /// 
    /// Multiple instances of this class can exist in the list of status effects at a time.
    /// 
    /// (I'd put some thought into this one, but I'm not sure what to do if there are different sources of slowed ...
    /// if a player is already slowed (80% speed reduction) and get hits with a different slow (30% speed reduction)
    /// would the new slow replace the old slow effect and now the player is moving faster? Will the existance of the old slow
    /// cause the new slow to be ignored? I'd gone with the DamageOverTime approach by having multiple instances exist, and 
    /// any slow statusEffect that match the slow statusEffect on the player will simply increase its duration.)
    /// 
    /// tl;dr slow can stack and if you don't like it, please tell me what happens when you're slowed 50% spd reduction and now you're hit with
    /// the slow status of 30% spd reduction
    /// 
    /// </summary>

    #region Accessors (c# Properties)

    public float PercentSpeedReduction { get { return m_percentSpeedReduction; } protected set { m_percentSpeedReduction = value; } }

    #endregion

    #region Variables

    protected bool m_isPlayer;
    protected float m_percentSpeedReduction;
    protected float m_percent;

    #endregion

    #region Player components

    protected PlayerMovement m_playerMoveScript;

    #endregion

    #region AI components

    protected EnemyMovement m_enemMovement;

    #endregion

    #region Functions

    public Slow(List<StatusEffect> parentList, string name, StatusType type, float duration, 
                float percentSpdReduction, PlayerMovement playerMove)
                 : base(parentList, name, type, duration)
    {
        // Slow effect for players
        m_isPlayer = true;
        m_percentSpeedReduction = percentSpdReduction;
        m_playerMoveScript = playerMove;

        m_percent = ((100 - m_percentSpeedReduction) / 100);

        // Apply slowdown
        m_playerMoveScript.AlterWalkSpeed(m_playerMoveScript.WalkSpeed * m_percent);
        m_playerMoveScript.AlterRunSpeed(m_playerMoveScript.RunSpeed * m_percent);
    }

    public Slow(List<StatusEffect> parentList, string name, StatusType type, float duration,
                float percentSpdReduction, EnemyMovement enemyMovement)
                 : base(parentList, name, type, duration)
    {
        // Slow effect for AI entities
        m_isPlayer = false;
        m_percentSpeedReduction = percentSpdReduction;
        m_enemMovement = enemyMovement;

        m_percent = ((100 - m_percentSpeedReduction) / 100);

        // Apply slowdown
        m_enemMovement.SetSpeed(m_enemMovement.Speed * m_percent);
    }

    /// <summary>
    /// Does nothing. The effect is already applied during the constructor so just do nothing.
    /// </summary>
    public override void DoStatusEffect()
    {
        return;
    }

    /// <summary>
    /// Remove this StatusEffect from a list of status effects, and reverts the slowdown.
    /// </summary>
    public override void Stop()
    {
        if (m_isPlayer)
        {
            // Revert slowdown
            m_playerMoveScript.AlterWalkSpeed(m_playerMoveScript.WalkSpeed / m_percent);
            m_playerMoveScript.AlterRunSpeed(m_playerMoveScript.RunSpeed / m_percent);
        }
        else
        {
            m_enemMovement.SetSpeed(m_enemMovement.Speed / m_percent);
        }

        base.Stop();
    }

    /// <summary>
    /// Decrease the duration by Time.deltaTime. If duration is zero, invoke Stop.
    /// </summary>
    public override void Tick()
    {
        m_duration -= Time.deltaTime;
        if (m_duration <= 0)
        {
            Stop();
        }
    }

    #endregion
}
