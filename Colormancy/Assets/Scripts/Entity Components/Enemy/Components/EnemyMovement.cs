using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(EnemyAnimationManager))]
[DisallowMultipleComponent]
public class EnemyMovement : MonoBehaviourPun, IPunObservable
{
    // This class is responsible for AI movement variables
    // The AI utilizes the Unity Navmesh and the Photon framework to sync its position
    // This class must remain enabled at all times, in order to sync the movement
    // in multiplayer.

    #region Accessors (c# Properties)

    // Movement accessors
    public float Speed { get { return m_speed; } protected set { m_speed = value; } }
    
    public Vector3 CurrentVelocity { get { return m_navMeshAgent.velocity; } protected set { m_navMeshAgent.velocity = value; } }

    public EnemyAnimationManager.EnemyState CurrentAnimState { get { return m_currentAnimState; } protected set { m_currentAnimState = value; } }

    public Vector3 DirectionToPlayer { get { return m_directionToPlayer; } protected set { m_directionToPlayer = value; } }
    public float AngleFromPlayer { get { return m_angleFromPlayer; } protected set { m_angleFromPlayer = value; } }
    public float DistanceFromPlayer { get { return m_distanceFromPlayer; } protected set { m_distanceFromPlayer = value; } }

    // Wander accessors
    public RangeTime WanderTime { get { return m_wanderTime; } protected set { m_wanderTime = value; } }
    public RangeTime IdleTime { get { return m_idleTime; } protected set { m_idleTime = value; } }

    public float WanderRadius { get { return m_wanderRadius; } protected set { m_wanderRadius = value; } }

    public Task WanderRandomDirectionTask { get { return m_wanderRandomDirectionTask; } protected set { m_wanderRandomDirectionTask = value; } }

    public WanderState CurrentWanderState { get { return m_wState; } protected set { m_wState = value; } }
    public WanderState LastWanderState { get { return m_lastWState; } protected set { m_lastWState = value; } }

    // Rigidbody accessors
    public float Mass { get { return m_rb.mass; } protected set { m_rb.mass = value; } } // mass of character

    #endregion

    #region Variables

    public enum WanderState
    {
        NotWandering,
        Idle,
        Wander
    }

    [System.Serializable]
    public class RangeTime
    {
        public float minTime = 2.0f;
        public float maxTime = 5.0f;
    }

    //Set to false false for enemies using their own animation system
    public bool SetAnims = true;

    // Syncing (photon) variables
    protected bool m_canInvokeMovementFunctions = true; // false if Photon.IsMasterClient is false
    protected bool m_hasReinitialized = true; // invoked whenever this client becomes new master client

    // Maximum time delay before we catch up to our position - Lower is stiffer
    // i.e: 0.5f means every half second the replica will have caught up to the master
    protected const float m_maxLerpTime = 0.2f;

    // Desired position given from network
    protected Vector3 m_latestPosition = Vector3.zero;

    // Where we were last time a new position was given
    protected Vector3 m_positionAtLastUpdate = Vector3.zero;

    // Desired rotation given from network
    protected Quaternion m_latestRotation = Quaternion.identity;

    //Where we were last time a new rotation was given
    protected Quaternion m_rotationAtLastUpdate = Quaternion.identity;

    protected float m_lerpTimer = 0;

    protected float m_navMeshLastSpeed = 0; // cache NavMeshSpeed so that we don't have to update the animation if its the same as the last frame

    // Movement variables

    [Tooltip("Speed of character")]
    [SerializeField] protected float m_speed = 18f;

    protected EnemyAnimationManager.EnemyState m_currentAnimState = EnemyAnimationManager.EnemyState.Idle;

    protected Vector3 m_directionToPlayer = Vector3.zero;
    protected float m_angleFromPlayer = 0;
    protected float m_distanceFromPlayer = -1;

    // Wander variables

    [SerializeField]
    protected RangeTime m_wanderTime;

    [SerializeField]
    protected RangeTime m_idleTime;

    [SerializeField]
    protected float m_wanderRadius = 10f;

    protected Task m_wanderRandomDirectionTask;

    protected WanderState m_wState = WanderState.NotWandering;
    protected WanderState m_lastWState = WanderState.NotWandering;

    #endregion

    #region Components

    protected GameObject m_character;
    protected NavMeshAgent m_navMeshAgent;
    protected EnemyAnimationManager m_animManager;
    protected Rigidbody m_rb;

    // These components will be reenable when this client becomes the master client
    // so that the AI will continue to work
    [SerializeField]
    protected EnemyChaserAI m_mainAIScript; // all "normal" AI scripts derive from EnemyChaserAI
    [SerializeField]
    protected EnemyTargeting m_enemTargeting;
    [SerializeField]
    protected PhotonView m_enemView;

    #endregion

    #region MonoBehaviour callbacks

    // Start is called before the first frame update
    protected void Start()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_animManager = GetComponent<EnemyAnimationManager>();
        m_rb = GetComponent<Rigidbody>();

        // Override the speed variable in m_navMeshAgent if it's not set already.
        m_navMeshAgent.speed = m_speed;

        // (important) Set up the wander Task, start it and then pause it
        // This is different than disabling autostart (because that implies you will start it later)
        m_wanderRandomDirectionTask = new Task(ShuffleRandomDirection());
        m_wanderRandomDirectionTask.Pause();

        m_canInvokeMovementFunctions = PhotonNetwork.IsMasterClient; // don't disable this class b/c we would like to sync the transforms and rotations
        m_hasReinitialized = !PhotonNetwork.IsMasterClient;
    }

    protected void Update()
    {
        if (!photonView.IsMine)
        {
            // do interpolation for other clients (not me)
            if (m_lerpTimer < m_maxLerpTime)
            {
                m_lerpTimer += Time.deltaTime;
                float t = Mathf.Clamp01(m_lerpTimer / m_maxLerpTime);
                transform.position = Vector3.Lerp(m_positionAtLastUpdate, m_latestPosition, t);
                transform.rotation = Quaternion.Lerp(m_rotationAtLastUpdate, m_latestRotation, t);
            }
        }

        if (PhotonNetwork.IsMasterClient)
        {
            if (m_navMeshLastSpeed != m_navMeshAgent.speed)
            {
                m_navMeshLastSpeed = m_navMeshAgent.velocity.magnitude / m_navMeshAgent.speed; // constrain to 0 -> 1 for blend tree animation
                if (SetAnims)
                {
                    m_animManager.SetSpeed(m_navMeshLastSpeed);
                }
            }
        }

        if (m_navMeshLastSpeed != m_navMeshAgent.speed)
        {
            // Always set the speed of the current character
            // If we're the masterClient, calculate the speed with the velocity from NavMesh
            // otherwise for other clients, m_navMeshLastSpeed would be updated by OnPhotonSerializeView
            if (PhotonNetwork.IsMasterClient)
            {
                m_navMeshLastSpeed = m_navMeshAgent.velocity.magnitude / m_navMeshAgent.speed; // constrain to 0 -> 1 for blend tree animation
            }
            if (SetAnims)
            {
                m_animManager.SetSpeed(m_navMeshLastSpeed);
            }
        }

        // check if we're now the master client
        if (m_enemView.Controller == PhotonNetwork.LocalPlayer && PhotonNetwork.IsMasterClient && !m_hasReinitialized)
        {
            m_hasReinitialized = true;

            //If the master client leaves, a new player will be assigned as a master client
            // Check to see if the current client is the master client, and so we'll reenable the AI scripts
            // enemyTargeting and the main AI script (EnemyChaser, EnemyRangedAI, etc)

            // Reactivate this AI for this client, and begin syncing to the other clients.
            m_canInvokeMovementFunctions = true;
            if (m_enemTargeting)
                m_enemTargeting.enabled = true;
            if (m_mainAIScript)
                m_mainAIScript.enabled = true;
            m_navMeshAgent.enabled = true;
        }
        else if (m_hasReinitialized)
        {
            // reset this value if we're no longer master client so that we are able to recieve the master client position later on
            m_hasReinitialized = false;
        }
    }

    #endregion

    #region Setters

    public void SetAngleFromPlayer(float newAngle)
    {
        m_angleFromPlayer = newAngle;
    }

    public void SetCurrentAnimState(EnemyAnimationManager.EnemyState newState)
    {
        m_currentAnimState = newState;
    }

    public void SetDistanceFromPlayer(float dist)
    {
        m_distanceFromPlayer = dist;
    }

    public void SetDirectionToPlayer(Vector3 newDir)
    {
        m_directionToPlayer = newDir;
    }

    /// <param name="newSpeed">New speed to override walkSpeed and NavMeshAgent speed</param>
    public void SetSpeed(float newSpeed)
    {
        m_speed = newSpeed;
        m_navMeshAgent.speed = m_speed;
    }

    #endregion

    #region Protected functions

    /// <summary>
    /// Returns a random valid position on the NavMesh. 
    /// </summary>
    /// <param name="center">The center of the place we start looking.</param>
    /// <param name="range">The radius around the center to check for a valid NavMesh.</param>
    /// <param name="result">The return value, a valid position on the scene's NavMesh.</param>
    /// <returns>Whether we found a valid random position on the NavMesh</returns>
    protected bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (IsPositionOnNavMesh(randomPoint, out hit))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    /// <summary>
    /// A function to make the enemy find a random valid position on the NavMesh and travel there, and then idle for a bit.
    /// Rinse and repeat.
    /// </summary>
    protected IEnumerator ShuffleRandomDirection()
    {
        while (true)
        {
            WanderToRandomDirection();
            yield return new WaitForSecondsRealtime(Random.Range(m_idleTime.minTime, m_idleTime.maxTime));

            WanderIdle();
            yield return new WaitForSecondsRealtime(Random.Range(m_wanderTime.minTime, m_wanderTime.maxTime));
        }
    }

    #endregion

    #region Public functions

    /// <summary>
    /// Disable the NavMeshAgent (and optionally enable the Rigidbody, or optionally enable the idleAnimation)
    /// </summary>
    public void DisableAgent(bool enableRigidbody = false, bool playIdleAnimation = false)
    {
        if (!m_canInvokeMovementFunctions) return;

        m_navMeshAgent.enabled = false;
        if (playIdleAnimation)
        {
            m_animManager.ChangeState(EnemyAnimationManager.EnemyState.Idle);
        }
        if (enableRigidbody && m_rb.isKinematic)
        {
            m_rb.isKinematic = false;
        }
    }

    /// <summary>
    /// Enable the NavMeshAgent and disables the Rigidbody if it is enabled.
    /// </summary>
    public void EnableAgent(bool disableRigidbody = false)
    {
        if (!m_canInvokeMovementFunctions) return;

        m_navMeshAgent.enabled = true;
        if (disableRigidbody && !m_rb.isKinematic)
        {
            m_rb.isKinematic = true;
        }
    }

    /// <summary>
    /// If we're currently wandering, stop that, and make the agent exit wandering mode.
    /// </summary>
    public void ExitWanderingMode(bool disableNavMeshAgent = false)
    {
        if (!m_canInvokeMovementFunctions) return;

        if (m_wState == WanderState.Wander || m_wState == WanderState.Idle)
        {
            if (disableNavMeshAgent)
            {
                m_navMeshAgent.isStopped = true;
            }
            m_wanderRandomDirectionTask.Pause();
            m_lastWState = m_wState;
            m_wState = WanderState.NotWandering;
        }
    }

    /// <summary>
    /// Rotates the character towards the player.
    /// </summary>
    public void FacePlayer()
    {
        if (!m_canInvokeMovementFunctions) return;

        // turn towards player when attacking
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_directionToPlayer), 0.1f);
    }

    /// <summary>
    /// Rotates the character towards the given target.
    /// </summary>
    public void FaceTarget(Vector3 directionToTarget)
    {
        if (!m_canInvokeMovementFunctions) return;

        // turn towards player when attacking
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToTarget), 0.1f);
    }

    /// <summary>
    /// Find a random position on the map that the NavMeshAgent can travel to.
    /// Search around the character with a radius of newRadius
    /// </summary>
    /// <param name="newRadius">The radius in which we search around our character for a random NavMesh position.</param>
    /// <returns>A valid random position on the NavMesh</returns>
    public Vector3 GetRandomPosition()
    {
        if (!m_canInvokeMovementFunctions) return Vector3.zero;

        Vector3 newPosition;
        while (true)
        {
            if (RandomPoint(transform.position, m_wanderRadius, out newPosition))
            {
                break;
            }
        }
        return newPosition;
    }

    /// <summary>
    /// Is the NavMeshAgent currently enabled?
    /// </summary>
    public bool IsAgentActive()
    {
        return m_navMeshAgent.enabled;
    }

    /// <summary>
    /// Is the player animation currently in the walk or run state?
    /// </summary>
    /// <returns>Returns true if the current animation state is walking or running</returns>
    public bool IsAgentMoving()
    {
        return m_currentAnimState == EnemyAnimationManager.EnemyState.Move;
    }

    /// <summary>
    /// Called whenever you want to utilize NavMeshAgent.Move without NavMeshAgent's SetDestination fighting against your manual movement.
    /// The AI's animation should be in the walk or run state before calling this.
    /// </summary>
    /// <returns> Is the AI in the walk or run state, and is the NavMeshAgent disabled (.isStopped is true)</returns>
    public bool IsManualMovementEnabled()
    {
        return IsAgentMoving() && m_navMeshAgent.isStopped;
    }

    /// <summary>
    /// Shoots a ray at the position to check if this position is on the level's NavMesh.
    /// </summary>
    /// <param name="position"></param>
    /// <returns>Whether the given position is on the level's NavMesh</returns>
    public bool IsPositionOnNavMesh(Vector3 position, out NavMeshHit hit)
    {
        if (NavMesh.SamplePosition(position, out hit, 1.0f, NavMesh.AllAreas))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Wrapper function for NavMeshAgent's Move
    /// </summary>
    public void ManuallyMove(Vector3 offset)
    {
        if (!m_canInvokeMovementFunctions) return;

        m_navMeshAgent.Move(offset);
    }

    /// <summary>
    /// Wrapper function for NavMeshAgent's SetDestination. Changed this to regular function so that the masterclient would control .SetDestination
    /// and not have this be calculated for every clietn
    /// </summary>
    public void MoveToPosition(Vector3 pos)
    {
        if (!m_canInvokeMovementFunctions) return;

        // don't check if the position is on navMesh before we even attempt moving there (this breaks stuff)
        try
        {
            m_navMeshAgent.SetDestination(pos);
        }
        catch { }
    }

    /// <summary>
    /// Calculate and return the quaternion that faces away from the player's position.
    /// </summary>
    /// <returns>The Quaternion that faces away from the direction to the player.</returns>
    public Quaternion OppositePlayerDirection()
    {
        if (!m_canInvokeMovementFunctions) return Quaternion.identity;
        return Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_directionToPlayer) * Quaternion.Euler(0, 180f, 0), 0.1f); ;
    }

    /// <summary>
    /// Wrapper function for Rigidbody AddForce(). This will only work if Rigidbody is in control instead of NavMeshAgent.
    /// </summary>
    /// <param name="force"></param>
    /// <param name="mode"></param>
    public void RigidbodyAddForce(Vector3 force, ForceMode mode)
    {
        if (!m_canInvokeMovementFunctions) return;
        if (!m_rb.isKinematic)
        {
            m_rb.AddForce(force, mode);
        }
    }

    public void SetNavMeshVelocity(Vector3 newVel)
    {
        if (!m_canInvokeMovementFunctions) return;
        m_navMeshAgent.velocity = newVel;
    }

    /// <summary>
    /// Start wandering if we're not wandering already. 
    /// </summary>
    public void StartWandering(bool enableNavMeshAgent = false)
    {
        if (!m_canInvokeMovementFunctions) return;
        if (m_wState == WanderState.NotWandering)
        {
            if (m_lastWState != WanderState.NotWandering)
            {
                m_wState = m_lastWState;
            }
            if (enableNavMeshAgent)
            {
                m_navMeshAgent.isStopped = false;
            }
            m_wanderRandomDirectionTask.Unpause();
        }
    }

    /// <summary>
    /// Stop all ongoing Tasks or coroutines. Then disables the script.
    /// </summary>
    public void StopAllTasks()
    {
        if (!m_canInvokeMovementFunctions) return;
        if (m_wanderRandomDirectionTask != null)
        {
            m_wanderRandomDirectionTask.Stop();
        }
        StopAllCoroutines();
        enabled = false;
    }

    /// <summary>
    /// If the NavMeshAgent is currently traveling to a destination ... stop it.
    /// </summary>
    public void StopMovingAndDontChangeAnimation()
    {
        if (!m_canInvokeMovementFunctions) return;

        if (m_navMeshAgent)
        {
            // Stop the agent from moving
            if (m_navMeshAgent.isOnNavMesh && IsPositionOnNavMesh(transform.position, out _))
            {
                MoveToPosition(transform.position); // set destination to current destination so it wont keep moving
            }
            m_navMeshAgent.velocity = Vector3.zero;
        }
    }

    /// <summary>
    /// Stop moving, but still stay in wandering mode, we're idling for now.
    /// </summary>
    public void WanderIdle()
    {
        if (!m_canInvokeMovementFunctions) return;
        StopMovingAndDontChangeAnimation();
        m_wState = WanderState.Idle;
        m_animManager.ChangeState(EnemyAnimationManager.EnemyState.Idle);
    }

    /// <summary>
    /// Move the agent to a random position on the NavMesh and set the Agent to the wander state.
    /// </summary>
    public void WanderToRandomDirection()
    {
        if (!m_canInvokeMovementFunctions) return;
        if (m_navMeshAgent && m_navMeshAgent.isOnNavMesh)
        {
            Vector3 ranPosition = GetRandomPosition();
            if (IsPositionOnNavMesh(ranPosition, out _))
            {
                //photonView.RPC("MoveToPosition", RpcTarget.All, ranPosition);
                MoveToPosition(ranPosition);
            }

            m_wState = WanderState.Wander;
            m_animManager.ChangeState(EnemyAnimationManager.EnemyState.Move);
        }
    }

    #endregion

    #region Photon functions

    // IPunObservable Implementation
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Masterclient syncs both position and rotation, and the current player's direciton

        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(m_directionToPlayer);
            stream.SendNext(m_navMeshLastSpeed);
        }
        else
        {
            //New position received
            //Reset timer and record positions to lerp between
            m_lerpTimer = 0;
            m_latestPosition = (Vector3)stream.ReceiveNext();
            m_latestRotation = (Quaternion)stream.ReceiveNext();
            m_positionAtLastUpdate = transform.position;
            m_rotationAtLastUpdate = transform.rotation;

            // Update the direction to the current player
            m_directionToPlayer = (Vector3)stream.ReceiveNext();
            m_navMeshLastSpeed = (float)stream.ReceiveNext();
        }
    }

    #endregion
}
