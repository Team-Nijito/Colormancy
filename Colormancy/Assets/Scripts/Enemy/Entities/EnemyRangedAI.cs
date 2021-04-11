using Photon.Pun;
using UnityEngine;

public class EnemyRangedAI : EnemyChaserAI
{
    // Similar to an EnemyChaser, but shoots projectiles at the players instead of meleeing

    // Note ; ranged enemies move closer if they miss
    // tried to make them reposition so they have a better chance
    // at hitting the target, but there are still bugs
    // (like the ranged enemies continuously firing... and not moving if player stay in place)
    // and the spawnpoint of the projectile being behind the skeleton

    #region Variables

    protected float m_tempAttackRange;
    protected EnemyProjectileAbility m_enemProjectile;

    #endregion

    #region MonoBehaviour callbacks

    protected override void Start()
    {
        m_enemProjectile = GetComponent<EnemyProjectileAbility>();
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
                    if (m_enemMovement.currentWanderState == EnemyMovement.WanderState.Wander)
                    {
                        m_enemMovement.RunOrWalkDependingOnSpeed();
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
            if (m_enemMovement.currentWanderState == EnemyMovement.WanderState.Wander)
            {
                m_enemMovement.RunOrWalkDependingOnSpeed();
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
            m_enemMovement.RunOrWalkDependingOnSpeed();
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
    /// I'm pretty sure this breaks some things, so it's unused until further testing / fixing.
    /// </summary>
    [PunRPC]
    private void RangeGetCloser()
    {
        m_enemTargeting.ChangeAttackStoppingRange(-1f, ref m_tempAttackRange);
    }

    /// <summary>
    /// (PunRPC) Move the AI away so that the AI would be farther away from player
    /// /// I'm pretty sure this breaks some things, so it's unused until further testing / fixing.
    /// </summary>
    [PunRPC]
    private void RangeGetFarther()
    {
        m_enemTargeting.ChangeAttackStoppingRange(0.1f, ref m_tempAttackRange);
    }

    #endregion
}
