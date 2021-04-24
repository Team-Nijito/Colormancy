using Photon.Pun;
using UnityEngine;

public class EnemyRangedAI : EnemyChaserAI
{
    // Similar to an EnemyChaser, but shoots projectiles at the players instead of meleeing
    #region Variables

    protected float m_tempAttackRange;
    protected EnemyProjectileAbility m_enemProjectile;

    #endregion

    #region MonoBehaviour callbacks

    protected override void Start()
    {
        base.Start();
        m_enemProjectile = GetComponent<EnemyProjectileAbility>();

        m_tempAttackRange = m_enemTargeting.AttackRange;
    }

    #endregion

    #region Protected functions

    /// <summary>
    /// Consider what the AI will do at any point, and handles AI animation
    /// </summary>
    protected override void ProcessAIIntent()
    {
        if (PhotonNetwork.InRoom && m_enemTargeting.TargetPlayer)
        {
            m_enemMovement.SetDirectionToPlayer(m_enemTargeting.TargetPlayer.position - transform.position);
            m_enemMovement.SetAngleFromPlayer(Vector3.Angle(m_enemMovement.DirectionToPlayer, transform.forward));

            if (TargetIsWithinCloseDetectionRadius())
            {
                // player got too close, they're detected
                // raycast here to see if there is an object between AI and player
                if (m_enemTargeting.CanSeePlayer())
                {
                    photonView.RPC("PlayerIsDetected", RpcTarget.All);
                }
            }
            else if (TargetIsWithinDetectionRadiusAndFieldOfView())
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

                if (m_enemTargeting.IsActivelyTargetingPlayer())
                {
                    // start forgetting the target, but still target them
                    // until AI completely forgets target
                    photonView.RPC("PlayerIsTargetedRanged", RpcTarget.All);
                }
                else
                {
                    // wander randomly if we don't sense nor remember player
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
                m_animManager.ChangeState(EnemyAnimationManager.EnemyState.Idle);
            }
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
            m_animManager.ChangeState(EnemyAnimationManager.EnemyState.Move);
        }
        else
        {
            m_animManager.ChangeState(EnemyAnimationManager.EnemyState.Attack);
        }
    }

    #endregion
}
