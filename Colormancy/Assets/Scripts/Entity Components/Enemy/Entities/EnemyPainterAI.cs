using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(EnemyPaintAbility))]
public class EnemyPainterAI : EnemyChaserAI
{
    // paints the floor and runs away from players, never attacks the player

    #region Variables

    EnemyPaintAbility m_enemPaintAbility;

    #endregion

    #region MonoBehaviourCallbacks

    protected override void Start()
    {
        base.Start();
        m_enemPaintAbility = GetComponent<EnemyPaintAbility>();
    }

    #endregion

    #region Protected functions

    // Consider what the AI will do at any point, and handles AI animation
    protected override void ProcessAIIntent()
    {
        if (PhotonNetwork.InRoom && m_enemTargeting.TargetPlayer)
        {
            m_enemMovement.SetDirectionToPlayer(m_enemTargeting.TargetPlayer.position - transform.position);
            m_enemMovement.SetAngleFromPlayer(Vector3.Angle(m_enemMovement.DirectionToPlayer, transform.forward));

            if (TargetIsWithinDetectionRadius())
            {
                Vector3 oldDirection = m_enemMovement.DirectionToPlayer;
                oldDirection.y = 0;
                m_enemMovement.SetDirectionToPlayer(oldDirection);

                m_animManager.ChangeState(EnemyAnimationManager.EnemyState.Move);
            }
            else
            {
                // wander so we can paint randomly around us
                if (m_enemMovement.CurrentWanderState == EnemyMovement.WanderState.Wander)
                {
                    m_animManager.ChangeState(EnemyAnimationManager.EnemyState.Move);
                }
                else
                {
                    m_animManager.ChangeState(EnemyAnimationManager.EnemyState.Idle);
                }
            }
        }
        else
        {
            // wander around in offline mode
            if (m_enemMovement.CurrentWanderState == EnemyMovement.WanderState.Wander)
            {
                m_animManager.ChangeState(EnemyAnimationManager.EnemyState.Move);
            }
            else
            {
                m_animManager.ChangeState(EnemyAnimationManager.EnemyState.Move);
            }
        }
    }

    // Primarily used for moving and attacking
    protected override void HandleAIIntent()
    {
        if (PhotonNetwork.InRoom && m_enemTargeting.TargetPlayer)
        {
            m_enemMovement.SetCurrentAnimState(m_animManager.GetCurrentState());
            if (TargetIsWithinDetectionRadius())
            {
                m_enemMovement.ExitWanderingMode(true);
                
                // Go the opposite direction of the player
                transform.rotation = m_enemMovement.OppositePlayerDirection();
            }
            else
            {
                m_enemMovement.StartWandering(true);
            }

            if (m_enemMovement.IsManualMovementEnabled())
            {
                m_enemMovement.ManuallyMove(transform.forward * m_enemMovement.Speed * Time.deltaTime);
            }
        }
        else
        {
            m_enemMovement.StartWandering();
        }
    }

    #endregion

    #region Public functions

    /// <summary>
    /// Stop all ongoing Tasks or coroutines.
    /// </summary>
    public override void StopAllTasks()
    {
        m_enemPaintAbility.StopAllTasks();
        base.StopAllTasks();
    }

    #endregion
}
