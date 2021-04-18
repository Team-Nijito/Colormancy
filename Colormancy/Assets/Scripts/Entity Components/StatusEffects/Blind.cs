using System.Collections.Generic;
using UnityEngine;

public class Blind : StatusEffect
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

    protected GameObject m_blindPanel;

    #endregion

    #region AI variables

    protected EnemyMovement m_enemMovement;
    protected EnemyTargeting m_enemTargetting;

    protected readonly float m_panicIntervalSeconds = 2f;
    protected readonly float m_idleIntervalSeconds = 0.5f;

    protected float m_panic;
    protected float m_idle;

    #endregion

    #region Functions

    public Blind(List<StatusEffect> parentList, string name, StatusType type, float duration, GameObject blindPanel)
                 : base(parentList, name, type, duration)
    {
        m_isPlayer = true;
        m_blindPanel = blindPanel;
        // Blind the player now
        m_blindPanel.SetActive(true);
    }

    public Blind(List<StatusEffect> parentList, string name, StatusType type, float duration, 
                 EnemyMovement enemMove, EnemyTargeting enemyTargeting)
                 : base(parentList, name, type, duration)
    {
        m_isPlayer = false;

        m_enemMovement = enemMove;
        m_enemTargetting = enemyTargeting;
        // Blind the AI
        m_enemTargetting.Blind();

        m_enemMovement.WanderToRandomDirection();
        m_panic = m_panicIntervalSeconds;
        m_idle = 0;
    }

    /// <summary>
    /// If Blind is applied to a player, do nothing.
    /// If Blind is applied to an AI entity, make sure to 
    /// </summary>
    public override void DoStatusEffect()
    {
        if (!m_isPlayer)
        {
            if (m_idle <= 0)
            {
                // We're in panic state.
                m_panic -= Time.deltaTime;
                if  (m_panic <= 0)
                {
                    m_idle = m_idleIntervalSeconds;
                    m_enemMovement.WanderIdle();
                }
            }
            else
            {
                // We're in the idle state.
                m_idle -= Time.deltaTime;
                if (m_idle <= 0)
                {
                    m_panic = m_panicIntervalSeconds;
                    m_enemMovement.WanderToRandomDirection();
                }
            }
        }
    }

    /// <summary>
    /// Remove this StatusEffect from a list of status effects, and reverts the blind.
    /// </summary>
    public override void Stop()
    {
        // Revert blind
        if (m_isPlayer)
        {
            m_blindPanel.SetActive(false);
        }
        else
        {
            m_enemTargetting.UnBlind();
            m_enemMovement.StartWandering();
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
        else if (!m_isPlayer)
        {
            DoStatusEffect();
        }
    }

    #endregion
}
