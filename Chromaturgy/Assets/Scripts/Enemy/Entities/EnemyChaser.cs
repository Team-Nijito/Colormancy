using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

[RequireComponent(typeof(PhotonView))]
[DisallowMultipleComponent]
public class EnemyChaser : MonoBehaviourPun
{
    // Handles the logic, and movement of the enemy chaser.

    #region Variables

    // Components
    protected EnemyMovement m_enemMovement;
    protected NavMeshAgent m_navMeshAgent;
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
        m_navMeshAgent = GetComponent<NavMeshAgent>();
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

        ProcessAIIntent();
    }

    protected virtual void FixedUpdate()
    {
        HandleAIIntent();
    }

    #endregion
    
    #region Private Methods

    /// <summary>
    /// Consider what the AI will do at any point, and handles AI animation
    /// </summary>
    protected virtual void ProcessAIIntent()
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
                    photonView.RPC("PlayerIsTargeted", RpcTarget.All);
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
    /// Primarily used for moving and attacking
    /// </summary>
    protected virtual void HandleAIIntent()
    {
        if (m_enemTargeting.TargetPlayer)
        {
            m_enemMovement.SetCurrentAnimState(m_animManager.GetCurrentState());

            if (Vector3.Distance(m_enemTargeting.TargetPlayer.position, transform.position) < m_enemTargeting.DetectionRadius)
            {
                EnemyAnimationManager.EnemyState state = m_enemMovement.CurrentAnimState;

                if (state == EnemyAnimationManager.EnemyState.Walk || state == EnemyAnimationManager.EnemyState.Run)
                {
                    m_navMeshAgent.SetDestination(m_enemTargeting.TargetPlayer.position);
                }
                else if (state == EnemyAnimationManager.EnemyState.Attack)
                {
                    // Stop the agent from moving
                    m_navMeshAgent.SetDestination(transform.position);
                    m_navMeshAgent.velocity = Vector3.zero;
                    
                    // turn towards player when attacking
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_enemMovement.DirectionToPlayer), 0.1f);
                }
            }
        }
    }

    #endregion
}
