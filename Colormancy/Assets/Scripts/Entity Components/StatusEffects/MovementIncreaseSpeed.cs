using System.Collections.Generic;
using UnityEngine;

public class MovementIncreaseSpeed : StatusEffect
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

    public float PercentSpeedAddition { get { return m_percentSpeedAddition; } protected set { m_percentSpeedAddition = value; } }

    #endregion

    #region Variables

    protected bool m_isPlayer;
    protected float m_percentSpeedAddition;
    protected float m_percent;

    #endregion

    #region Player components

    protected PlayerMovement m_playerMoveScript;

    #endregion

    #region Functions

    public MovementIncreaseSpeed(List<StatusEffect> parentList, StatusType type, float duration, string source,
                float value, PlayerMovement playerMove)
                 : base(parentList, type, duration, source)
    {
        // Slow effect for players
        m_isPlayer = true;
        m_percentSpeedAddition = value;
        m_playerMoveScript = playerMove;

        m_percent = ((100 + m_percentSpeedAddition) / 100);

        // Apply slowdown
        m_playerMoveScript.AlterWalkSpeed(m_playerMoveScript.WalkSpeed * m_percent);
        m_playerMoveScript.AlterRunSpeed(m_playerMoveScript.RunSpeed * m_percent);
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
        m_playerMoveScript.AlterWalkSpeed(m_playerMoveScript.WalkSpeed / m_percent);
        m_playerMoveScript.AlterRunSpeed(m_playerMoveScript.RunSpeed / m_percent);

        base.Stop();
    }

    public override void Update()
    {

    }

    #endregion
}
