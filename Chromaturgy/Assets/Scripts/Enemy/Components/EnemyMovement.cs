using Photon.Pun;
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

    #region Accessors (c# Properties)

    // Movement accessors
    public float Speed { get { return m_speed; } protected set { m_speed = value; } }
    public float SpeedTriggerRun { get { return m_speedTriggerRun; } protected set { m_speedTriggerRun = value; } }
    
    public EnemyAnimationManager.EnemyState CurrentAnimState { get { return m_currentAnimState; } protected set { m_currentAnimState = value; } }

    public Vector3 DirectionToPlayer { get { return m_directionToPlayer; } protected set { m_directionToPlayer = value; } }
    public float AngleFromPlayer { get { return m_angleFromPlayer; } protected set { m_angleFromPlayer = value; } }
    public float DistanceFromPlayer { get { return m_distanceFromPlayer; } protected set { m_distanceFromPlayer = value; } }

    // Wander accessors
    public RangeTime WanderTime { get { return m_wanderTime; } protected set { m_wanderTime = value; } }
    public RangeTime IdleTime { get { return m_idleTime; } protected set { m_idleTime = value; } }

    public float WanderRadius { get { return m_wanderRadius; } protected set { m_wanderRadius = value; } }

    public Task WanderRandomDirectionTask { get { return m_wanderRandomDirectionTask; } protected set { m_wanderRandomDirectionTask = value; } }

    public WanderState currentWanderState { get { return m_wState; } protected set { m_wState = value; } }
    public WanderState lastWanderState { get { return m_lastWState; } protected set { m_lastWState = value; } }

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

    // Movement variables

    [Tooltip("Speed of character")]
    [SerializeField] protected float m_speed = 18f;

    [Tooltip("The speed at which the Run animation is triggered")]
    [SerializeField] protected float m_speedTriggerRun = 15f;

    [Tooltip("Time that the Rigidbody controls the object whenever the NavMeshAgent is disabled (specifically for knockback)")]
    [SerializeField] protected float m_durationRigidbody = 0.75f; 

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
    protected Task m_moveErraticallyTask;

    protected WanderState m_wState = WanderState.NotWandering;
    protected WanderState m_lastWState = WanderState.NotWandering;

    #endregion

    #region Components

    protected GameObject m_character;
    protected Rigidbody m_rigidBody;
    protected NavMeshAgent m_navMeshAgent;
    protected EnemyAnimationManager m_animManager;

    #endregion

    #region MonoBehaviour callbacks

    // Start is called before the first frame update
    protected void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_animManager = GetComponent<EnemyAnimationManager>();

        // Override the speed variable in m_navMeshAgent if it's not set already.
        m_navMeshAgent.speed = m_speed;
        
        // (important) Set up the wander Task, start it and then pause it
        // This is different than disabling autostart (because that implies you will start it later)
        m_wanderRandomDirectionTask = new Task(ShuffleRandomDirection());
        m_wanderRandomDirectionTask.Pause();
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

    #endregion

    #region Protected functions

    /// <summary>
    /// A variant of ShuffleRandomDirection where the AI would "panic" (move around randomly at frequent intervals).
    /// </summary>
    /// <param name="duration">Duration of panic (Min val = 1f)</param>
    protected IEnumerator PanicRandomDirection(float duration)
    {
        float cumulativeTime = 0f;

        while (cumulativeTime < duration)
        {
            if (m_navMeshAgent.isOnNavMesh)
            {
                m_navMeshAgent.SetDestination(GetRandomPosition()); // choose random direction
            }
            m_wState = WanderState.Wander;
            yield return new WaitForSecondsRealtime(0.75f);
            if (m_navMeshAgent.isOnNavMesh)
            {
                m_navMeshAgent.SetDestination(transform.position); // set destination to current destination so it wont keep moving
            }
            m_wState = WanderState.Idle;
            yield return new WaitForSecondsRealtime(0.25f);

            cumulativeTime += 1f;
        }
    }

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
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    /// <summary>
    /// When called, the Rigidbody would control this object instead of the NavMeshAgent for a period of time.
    /// </summary>
    /// <param name="seconds">The time that the Rigidbody would manage this object</param>
    protected IEnumerator RigidbodyControlsObject(Vector3 initalDir, float force, float duration, bool stun)
    {
        // Disable the agent
        m_navMeshAgent.enabled = false;
        m_rigidBody.isKinematic = false;

        if (stun)
        {
            m_rigidBody.velocity = Vector3.zero;
        }
        else
        {
            // Apply the force
            m_rigidBody.AddForce(initalDir * force, ForceMode.Acceleration);
        }

        yield return new WaitForSecondsRealtime(duration);

        // Re-enable the agent
        m_navMeshAgent.enabled = true;
        m_rigidBody.isKinematic = true;
    }

    /// <summary>
    /// A function to make the enemy find a random valid position on the NavMesh and travel there, and then idle for a bit.
    /// Rinse and repeat.
    /// </summary>
    protected IEnumerator ShuffleRandomDirection()
    {
        while (true)
        {
            m_navMeshAgent.SetDestination(GetRandomPosition()); // choose random direction
            m_wState = WanderState.Wander;
            yield return new WaitForSecondsRealtime(Random.Range(m_idleTime.minTime, m_idleTime.maxTime));

            m_navMeshAgent.SetDestination(transform.position); // set destination to current destination so it wont keep moving
            m_wState = WanderState.Idle;
            yield return new WaitForSecondsRealtime(Random.Range(m_wanderTime.minTime, m_wanderTime.maxTime));
        }
    }

    protected IEnumerator Slowdown(float percentReductionSpeed, float duration)
    {
        float percent = ((100 - percentReductionSpeed) / 100);
        // note that m_speed doesn't affect the speed of the enemy (controlled by NavMeshAgent)
        // it's just to ensure that the value is accurate if its accessor is called during this
        // slowdown effect

        // Apply speed reduction
        m_speed *= percent; 
        m_navMeshAgent.speed *= percent;
        yield return new WaitForSecondsRealtime(duration);
        // Revert speed reduction
        m_speed /= percent;
        m_navMeshAgent.speed /= percent;
    }

    #endregion

    #region Public functions

    /// <summary>
    /// Apply a force on the character. This is performed by disabling
    /// the NavMeshAgent and letting the Rigidbody take over, then we wait
    /// for a duration until we let the NavMeshAgent take over again.
    /// </summary>
    /// <param name="offset">Apply a force in the direction of the offset.</param>
    public void ApplyForce(Vector3 dir, float force)
    {
        StartCoroutine(RigidbodyControlsObject(dir, force, m_durationRigidbody, false));
    }

    /// <summary>
    /// Slows the player down for a period of time.
    /// </summary>
    /// <param name="percentReduction">The reduction in speed.</param>
    /// <param name="duration">The duration that this slowdown will last.</param>
    public void ApplySlowdown(float percentReduction, float duration)
    {
        StartCoroutine(Slowdown(percentReduction, duration));
    }

    /// <summary>
    /// Stuns a character and prevent them from moving.
    /// </summary>
    /// <param name="duration">How long the stun will last.</param>
    public void ApplyStun(float duration)
    {
        StartCoroutine(RigidbodyControlsObject(Vector3.zero, 0, duration, true));
    }

    /// <summary>
    /// The character will now shuffle around aggressively.
    /// </summary>
    /// <param name="duration">Duration of blind panic.</param>
    public void BlindPanic(float duration)
    {
        if (m_moveErraticallyTask == null)
        {
            m_moveErraticallyTask = new Task(PanicRandomDirection(duration));
        }
        else
        {
            m_moveErraticallyTask.Stop();
            m_moveErraticallyTask = new Task(PanicRandomDirection(duration));
        }
    }

    /// <summary>
    /// Rotates the character towards the player.
    /// </summary>
    public void FacePlayer()
    {
        // turn towards player when attacking
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_directionToPlayer), 0.1f);
    }

    /// <summary>
    /// Find a random position on the map that the NavMeshAgent can travel to.
    /// Search around the character with a radius of newRadius
    /// </summary>
    /// <param name="newRadius">The radius in which we search around our character for a random NavMesh position.</param>
    /// <returns>A valid random position on the NavMesh</returns>
    public Vector3 GetRandomPosition()
    {
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
        return m_currentAnimState == EnemyAnimationManager.EnemyState.Walk || m_currentAnimState == EnemyAnimationManager.EnemyState.Run;
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
    /// Wrapper function for NavMeshAgent's Move
    /// </summary>
    public void ManuallyMove(Vector3 offset)
    {
        m_navMeshAgent.Move(offset);
    }

    /// <summary>
    /// Wrapper function for NavMeshAgent's SetDestination.
    /// </summary>
    public void MoveToPosition(Vector3 pos)
    {
        m_navMeshAgent.SetDestination(pos);
    }

    /// <summary>
    /// Calculate and return the quaternion that faces away from the player's position.
    /// </summary>
    /// <returns>The Quaternion that faces away from the direction to the player.</returns>
    public Quaternion OppositePlayerDirection()
    {
        return Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_directionToPlayer) * Quaternion.Euler(0, 180f, 0), 0.1f); ;
    }

    /// <summary>
    /// Change the AI animation depending on its speed values.
    /// </summary>
    public void RunOrWalkDependingOnSpeed()
    {
        if (m_speed > m_speedTriggerRun)
        {
            m_animManager.ChangeState(EnemyAnimationManager.EnemyState.Run);
        }
        else
        {
            m_animManager.ChangeState(EnemyAnimationManager.EnemyState.Walk);
        }
    }

    /// <summary>
    /// Start wandering if we're not wandering already. 
    /// </summary>
    public void StartWandering(bool enableNavMeshAgent = false)
    {
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
    /// If the NavMeshAgent is currently traveling to a destination ... stop it.
    /// </summary>
    public void StopMoving()
    {
        // Stop the agent from moving
        m_navMeshAgent.SetDestination(transform.position);
        m_navMeshAgent.velocity = Vector3.zero;
    }

    /// <summary>
    /// If we're currently wandering, stop that. 
    /// </summary>
    public void StopWandering(bool disableNavMeshAgent = false)
    {
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

    #endregion

    #region Photon functions

    // IPunObservable Implementation
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Use this information to sync child rotational transform
        // instead of placing PhotonView and PhotonTransfromView on child object
        if (stream.IsWriting)
        {
            if (m_character)
            {
                stream.SendNext(m_character.transform.localRotation);
            }
        }
        else
        {
            if (m_character)
            {
                m_character.transform.localRotation = (Quaternion)stream.ReceiveNext();
            }
        }
    }

    #endregion
}
