using UnityEngine;
using Photon.Pun;
using System;

[RequireComponent(typeof(PhotonView))]
[DisallowMultipleComponent]
public class EnemyChaserAI : MonoBehaviourPun, IEnemyDetection, IStatusEffects
{
    // Handles the logic, and movement of the enemy chaser.

    #region Variables

    // Components
    protected EnemyMovement m_enemMovement;
    protected HealthScript m_hscript;
    protected EnemyAnimationManager m_animManager;
    protected EnemyTargeting m_enemTargeting;
    protected EnemyHurtbox m_enemHurtbox;

    #endregion

    #region MonoBehaviour callbacks

    // Start is called before the first frame update
    protected virtual void Start()
    {
        m_hscript = GetComponent<HealthScript>();
        m_animManager = GetComponent<EnemyAnimationManager>();
        m_enemMovement = GetComponent<EnemyMovement>();
        m_enemHurtbox = GetComponent<EnemyHurtbox>();
        m_enemTargeting = GetComponent<EnemyTargeting>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        float playerDistance = m_enemMovement.DistanceFromPlayer;
        Transform target = m_enemTargeting.DetermineTargetPlayer(ref playerDistance);
        m_enemMovement.SetDistanceFromPlayer(playerDistance);

        if (target)
        {
            PhotonView targetPhotonView = target.gameObject.GetPhotonView();
            if (!m_enemTargeting.TargetPlayer || m_enemTargeting.TargetPlayer.gameObject.GetPhotonView().ViewID != targetPhotonView.ViewID)
            {
                // Only send another RPC, if the target this time is different.
                // Also check if player is still here
                if (targetPhotonView.Owner != null && targetPhotonView.Owner.TagObject != null)
                {
                    photonView.RPC("DeclareTargetPlayer", RpcTarget.All, target.gameObject.GetPhotonView().ViewID);
                }
            }
        }

        if (m_enemMovement.IsAgentActive() && !m_enemTargeting.IsBlind())
        {
            ProcessAIIntent();
        }
    }

    protected virtual void FixedUpdate()
    {
        if (m_enemMovement.IsAgentActive() && !m_enemTargeting.IsBlind())
        {
            HandleAIIntent();
        }
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Consider what the AI will do at any point, and handles AI animation
    /// </summary>
    protected virtual void ProcessAIIntent()
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
                    // still remember target, go after them and start forgetting the player
                    photonView.RPC("StartForgettingTask", RpcTarget.All);
                }

                if (m_enemTargeting.IsActivelyTargetingPlayer())
                {
                    // start forgetting the target, but still target them
                    // until AI completely forgets target
                    photonView.RPC("PlayerIsTargeted", RpcTarget.All);
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
    /// Primarily used for moving and attacking
    /// </summary>
    protected virtual void HandleAIIntent()
    {
        if (PhotonNetwork.InRoom && m_enemTargeting.TargetPlayer)
        {
            m_enemMovement.SetCurrentAnimState(m_animManager.GetCurrentState());

            if (m_enemTargeting.IsActivelyTargetingPlayer())
            {
                m_enemMovement.StopWandering();

                if (m_enemMovement.IsAgentMoving())
                {
                    m_enemMovement.MoveToPosition(m_enemTargeting.TargetPlayer.position);
                }
                else if (m_enemMovement.CurrentAnimState == EnemyAnimationManager.EnemyState.Attack)
                {
                    m_enemMovement.StopMoving();
                    m_enemMovement.FacePlayer();
                }
            }
            else
            {
                m_enemMovement.StartWandering();
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
    /// Character is blind, meaning that it will shuffle around aggressively and cannot target player.
    /// </summary>
    /// <param name="duration">Duration of blind</param>
    [PunRPC]
    public void ApplyBlind(float duration)
    {
        m_enemMovement.BlindPanic(duration);
        m_enemTargeting.Blind(duration);
    }

    /// <summary>
    /// (PunRPC) Apply a force to this character.
    /// </summary>
    /// <param name="dir">The direction of the force</param>
    /// <param name="force">The magnitude of the force</param>
    [PunRPC]
    public void ApplyForce(Vector3 dir, float force, float stunDuration)
    {
        m_enemMovement.ApplyForce(dir, force, stunDuration);
    }

    /// <summary>
    /// (PunRPC) Apply a slowdown to this character for a duration, then changes the character's speed to its speed before the slowdown. Stackable.
    /// </summary>
    /// <param name="percentReduction">Range(0,100f). What percentage will we reduce the character's speed by? 50%?</param>
    /// <param name="duration">How long the slowdown will last.</param>
    [PunRPC]
    public void ApplySlowdown(float percentReduction, float duration)
    {
        m_enemMovement.ApplySlowdown(percentReduction, duration);
    }

    /// <summary>
    /// (PunRPC) Stuns a character and prevent them from moving.
    /// </summary>
    /// <param name="duration">How long the stun will last.</param>
    [PunRPC]
    public void ApplyStun(float duration)
    {
        m_enemMovement.ApplyStun(duration);
    }

    /// <summary>
    /// Verbose detection function meant to improve readability.
    /// </summary>
    public virtual bool TargetIsWithinCloseDetectionRadius()
    {
        return m_enemMovement.DistanceFromPlayer < m_enemTargeting.CloseDetectionRadius;
    }

    /// <summary>
    /// Verbose detection function meant to improve readability.
    /// </summary>
    public virtual bool TargetIsWithinDetectionRadius()
    {
        return m_enemMovement.DistanceFromPlayer < m_enemTargeting.DetectionRadius;
    }

    /// <summary>
    /// Verbose detection function meant to improve readability.
    /// </summary>
    public virtual bool TargetIsWithinDetectionRadiusAndFieldOfView()
    {
        return TargetIsWithinDetectionRadius() && m_enemMovement.AngleFromPlayer < m_enemTargeting.FieldOfView;
    }

    /// <summary>
    /// Stop all ongoing Tasks or coroutines.
    /// </summary>
    public virtual void StopAllTasks()
    {
        m_enemTargeting.StopAllTasks();
        m_enemMovement.StopAllTasks();
    }

    #endregion
}
