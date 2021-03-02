using Photon.Pun;
using UnityEngine;

public class EnemyRanged : EnemyChaser
{
    // Similar to an EnemyChaser, but shoots projectiles at the players instead of meleeing

    // Note ; ranged enemies move closer if they miss
    // tried to make them reposition so they have a better chance
    // at hitting the target, but there are still bugs
    // (like the ranged enemies continuously firing... and not moving if player stay in place)
    // and the spawnpoint of the projectile being behind the skeleton
    
    #region Variables

    protected float m_tempAttackRange;

    protected EnemyProjectile m_enemProjectile;

    #endregion

    #region MonoBehaviour callbacks

    protected override void Start()
    {
        m_enemProjectile = GetComponent<EnemyProjectile>();
        m_enemTargeting = GetComponent<EnemyTargeting>();

        m_tempAttackRange = m_enemTargeting.AttackRange;
        base.Start();
    }

    #endregion

    #region Protected functions

    /// <summary>
    /// Consider what the AI will do at any point, and handles AI animation
    /// </summary>
    protected override void ProcessAIIntent()
    {
        if (m_enemTargeting.TargetPlayer)
        {
            m_enemMovement.SetDirectionToPlayer(m_enemTargeting.TargetPlayer.position - transform.position);
            m_enemMovement.SetAngleFromPlayer(Vector3.Angle(m_enemMovement.DirectionToPlayer, transform.forward));

            if (m_enemMovement.DistanceFromPlayer < m_enemTargeting.CloseDetectionRadius)
            {
                // player got too close, they're detected
                // raycast here to see if there is an object between AI and player
                if (m_enemTargeting.CanSeePlayer())
                {
                    photonView.RPC("PlayerIsDetected", RpcTarget.All);
                }
            }
            else if (m_enemMovement.DistanceFromPlayer < m_enemTargeting.DetectionRadius && m_enemMovement.AngleFromPlayer < m_enemTargeting.FieldOfView)
            {
                // player is in cone vision
                // raycast here to see if there is an object between AI and player
                if (m_enemTargeting.CanSeePlayer())
                {
                    photonView.RPC("PlayerIsDetected", RpcTarget.All);
                }
            }
            else
            {
                if (m_enemTargeting.RememberTarget)
                {
                    // still remember target, go after them
                    photonView.RPC("StartForgettingTask", RpcTarget.All);
                }

                if (m_enemTargeting.RememberTarget || m_enemTargeting.IsForgettingTarget)
                {
                    // start forgetting the target, but still target them
                    // until AI completely forgets target
                    photonView.RPC("PlayerIsTargetedRanged", RpcTarget.All);
                }
                else
                {
                    // don't see player, just idle for now
                    m_animManager.ChangeState(EnemyAnimationManager.EnemyState.Idle);
                }
            }
        }
        else
        {
            m_animManager.ChangeState(EnemyAnimationManager.EnemyState.Idle);
        }
    }

    /// <summary>
    /// (PunRPC) This is invoked in ProcessAIIntent whenever a player is considered "remembered" instead of "detected"
    /// and is also invoked by PlayerIsDetected()
    /// </summary>
    /// 
    [PunRPC]
    protected void PlayerIsTargetedRanged()
    {
        if (m_enemMovement.DirectionToPlayer.magnitude > m_tempAttackRange)
        {
            if (m_enemMovement.Speed > m_enemMovement.SpeedTriggerRun)
            {
                m_animManager.ChangeState(EnemyAnimationManager.EnemyState.Run);
            }
            else
            {
                m_animManager.ChangeState(EnemyAnimationManager.EnemyState.Walk);
            }
        }
        else
        {
            m_animManager.ChangeState(EnemyAnimationManager.EnemyState.Attack);
        }
    }

    #endregion

    #region Private functions

    /// <summary>
    /// (PunRPC) Move the AI closer so that the AI would have a better chance at hitting the target (hypothetically)
    /// </summary>
    [PunRPC]
    private void RangeGetCloser()
    {
        ChangeAttackStoppingRange(-1);
    }

    /// <summary>
    /// (PunRPC) Move the AI away so that the AI would be farther away from player
    /// </summary>
    [PunRPC]
    private void RangeGetFarther()
    {
        ChangeAttackStoppingRange(0.1f);
    }

    /// <summary>
    /// Change the attack and stopping range so that the AI would move closer to / farther away from the player.
    /// </summary>
    /// <param name="changeVal">How much to decrease the range by</param>
    private void ChangeAttackStoppingRange(float changeVal)
    {
        if (changeVal < 0)
        {
            // decrease range
            if ((m_navMeshAgent.stoppingDistance + changeVal) >= m_enemTargeting.CloseDetectionRadius)
            {
                m_tempAttackRange += changeVal;
                m_navMeshAgent.stoppingDistance += changeVal;
            }
        }
        else
        {
            // increase range
            if ((m_tempAttackRange + changeVal) < m_enemTargeting.AttackRange)
            {
                m_tempAttackRange += changeVal;
                m_navMeshAgent.stoppingDistance += changeVal;

                m_navMeshAgent.Move((transform.position - m_enemTargeting.TargetPlayer.position).normalized * changeVal);
            }
        }
    }

    #endregion
}
