using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(EnemyAnimationManager))]
[RequireComponent(typeof(EnemyMovement))]
[DisallowMultipleComponent]
public class EnemyTargeting : MonoBehaviourPun
{
    // This class is responsible for AI "vision" (raycasting) and player targetting

    #region Accessors (c# Properties)
    
    public Transform TargetPlayer { get { return m_targetPlayer; } protected set { m_targetPlayer = value;  } }
    public LayerMask IgnoreOtherEnemies { get { return m_ignoreOtherEnemiesLayer; } protected set { m_ignoreOtherEnemiesLayer = value; } }
    
    public float CloseDetectionRadius { get { return m_closeDetectionRadius; } protected set { m_closeDetectionRadius = value; } }
    public float DetectionRadius { get { return m_detectionRadius; } protected set { m_detectionRadius = value; } }
    public float FieldOfView { get { return m_fieldOfView; } protected set { m_fieldOfView = value; } }
    public float AttackRange { get { return m_attackRange; } protected set { m_attackRange = value; } }
    public float RememberTargetDuration { get { return m_rememberTargetDuration; } protected set { m_rememberTargetDuration = value; } }
    
    public bool RememberTarget { get { return m_rememberTarget; } protected set { m_rememberTarget = value; } }
    public bool IsForgettingTarget { get { return m_isForgettingTarget; } protected set { m_isForgettingTarget = value; } }

    public Coroutine ForgettingTargetCoroutineRef { get { return m_forgettingTargetCoroutineRef; } protected set { m_forgettingTargetCoroutineRef = value; } }

    #endregion

    #region Variables

    protected Transform m_targetPlayer;

    [SerializeField] protected LayerMask m_ignoreOtherEnemiesLayer;

    [SerializeField] protected float m_closeDetectionRadius = 1.5f; // used when a player gets too close to an enemy
    [SerializeField] protected float m_detectionRadius = 30f; // used in every other case

    [Range(0, 180)]
    [SerializeField] protected float m_fieldOfView = 50f; // degrees

    [SerializeField] protected float m_attackRange = 3f;

    [SerializeField] protected float m_rememberTargetDuration = 7f;  // how long does an AI "remember" a target once target is out of vision

    protected bool m_rememberTarget = false;
    protected bool m_isForgettingTarget = false;

    protected Coroutine m_forgettingTargetCoroutineRef = null;

    protected delegate void FunctionInvokeOnPlayerTargetted();

    #endregion

    #region Components

    protected EnemyAnimationManager m_animManager;
    protected EnemyMovement m_enemMovement;
    protected NavMeshAgent m_navMeshAgent;

    #endregion

    #region MonoBehaviour callbacks

    // Start is called before the first frame update
    protected void Start()
    {
        m_animManager = GetComponent<EnemyAnimationManager>();
        m_enemMovement = GetComponent<EnemyMovement>();
        m_navMeshAgent = GetComponent<NavMeshAgent>();

        m_ignoreOtherEnemiesLayer = LayerMask.GetMask("Ignore Raycast"); // obviously change this once we get a dedicated layer for enemies

        // Override the variables in m_navMeshAgent if they're not set already.
        m_navMeshAgent.stoppingDistance = m_attackRange;
    }

    #endregion

    #region Protected functions

    /// <summary>
    /// Allows the AI to remember and target the player until the player remains out of sight for m_rememberTargetDuration seconds.
    /// </summary>
    protected IEnumerator ForgetTargetAfterDuration()
    {
        m_rememberTarget = false;
        m_isForgettingTarget = true;
        yield return new WaitForSeconds(m_rememberTargetDuration);

        m_isForgettingTarget = false;

        if (m_navMeshAgent.isOnNavMesh)
        {
            // stop moving towards target
            m_navMeshAgent.SetDestination(transform.position);
        }
    }

    /// <summary>
    /// (PunRPC) This is invoked in ProcessAIIntent whenever a player is considered "detected"
    /// </summary>
    [PunRPC]
    protected void PlayerIsDetected()
    {
        Vector3 oldDirection = m_enemMovement.DirectionToPlayer;
        oldDirection.y = 0;
        m_enemMovement.SetDirectionToPlayer(oldDirection);

        // Reset the m_rememberTarget task
        m_rememberTarget = true;
        if (m_forgettingTargetCoroutineRef != null)
        {
            StopCoroutine(m_forgettingTargetCoroutineRef);
        }

        PlayerIsTargeted();
    }
    
    /// <summary>
    /// (PunRPC) This is invoked in ProcessAIIntent whenever a player is considered "remembered" instead of "detected"
    /// and is also invoked by PlayerIsDetected()
    /// </summary>
    [PunRPC]
    protected virtual void PlayerIsTargeted()
    {
        if (m_enemMovement.DirectionToPlayer.magnitude > m_attackRange)
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

    /// <summary>
    /// (PunRPC) Starts the forgetting task.
    /// </summary>
    [PunRPC]
    protected void StartForgettingTask()
    {
        m_forgettingTargetCoroutineRef = StartCoroutine(ForgetTargetAfterDuration());
    }

    #endregion

    #region Public functions

    /// <summary>
    /// The AI raycast to see if there is any obstacle between the AI and a player target.
    /// The ray currently originates from the chest of the AI.
    /// </summary>
    /// <returns>Returns true if there is no obstacle between the transform and the target.</returns>
    public bool CanSeePlayer()
    {
        bool canSee = false;
        Ray ray = new Ray(transform.position, m_targetPlayer.transform.position - transform.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, ~m_ignoreOtherEnemiesLayer))
        {
            if (hit.transform == m_targetPlayer)
            {
                canSee = true;
            }
        }
        return canSee;
    }

    // How to pass GameObject/transform through RPC: https://forum.unity.com/threads/how-to-photon-networking-send-gameobjects-transforms-and-other-through-the-network.343973/
    /// <summary>
    /// (PunRPC) This is used so that the Enemy's target is synced acrossed all players
    /// </summary>
    /// <param name="playerPhotonViewID">Player's photon view</param>
    [PunRPC]
    public void DeclareTargetPlayer(int playerPhotonViewID)
    {
        m_rememberTarget = false; // forget old player
        Transform playerTransform = PhotonView.Find(playerPhotonViewID).transform;
        m_targetPlayer = playerTransform;
    }


    /// <summary>
    /// Determine which player to seek out (if omnipotent), or just check
    /// if there are any players around the enemy (if not omnipotent)
    /// right now just omnipotent b/c we're in an arena of course they know we're here
    /// </summary>
    /// <param name="distanceFromPlayer"></param>
    /// <returns>The transform of the closest player in the server, or null if no players characters exist</returns>
    public virtual Transform DetermineTargetPlayer(ref float distanceFromPlayer)
    {
        Transform targetTransform = null;
        float targetDistance = -1;

        // Focus on the closest players from all players
        foreach (Player play in PhotonNetwork.PlayerList)
        {
            GameObject playObj = play.TagObject as GameObject;
            if (playObj)
            {
                float tmpDistance = Vector3.Distance(playObj.transform.position, transform.position);
                if (targetTransform)
                {
                    if (tmpDistance < Vector3.Distance(targetTransform.position, transform.position))
                    {
                        targetTransform = playObj.transform;
                        targetDistance = tmpDistance;
                    }
                }
                else
                {
                    targetTransform = playObj.transform;
                    targetDistance = tmpDistance;
                }
            }
        }
        distanceFromPlayer = targetDistance;
        return targetTransform;
    }

    /// <summary>
    /// Is the AI actively seeking out the position of the player at this moment?
    /// </summary>
    public bool IsActivelyTargetingPlayer()
    {
        return m_rememberTarget || m_isForgettingTarget;
    }
    
    #endregion
}
