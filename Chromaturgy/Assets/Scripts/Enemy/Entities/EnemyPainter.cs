using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine.AI;

public class EnemyPainter : EnemyChase
{
    // paints the floor and runs away from players, never attacks the player

    [System.Serializable]
    private class RangeTime
    {
        public float minTime = 3.0f;
        public float maxTime = 7.0f;
    }
    
    private enum WanderState
    {
        NotWandering,
        Idle,
        Wander
    }

    // Painting settings
    [SerializeField]
    private Color m_colorToPaint = Color.red;

    [SerializeField]
    private bool m_isUnpainter = true; // normally unpaints instead of paint

    [SerializeField]
    private float m_paintCooldown = 2f; // interval between "paints"

    [SerializeField]
    private float m_paintRadius = 3f;

    [SerializeField]
    private float m_raycastFloor = 1.5f; // how large is the laser we shoot downwards to check for a ground

    private RaycastHit m_raycastHit;
    private int m_paintableMask = 1 << 8; // only focus on the paintable mask

    // Wandering and idle settings
    [SerializeField]
    private RangeTime m_wanderTime;

    [SerializeField]
    private RangeTime m_idleTime;

    [SerializeField]
    private float m_wanderRadius = 10f;

    private Task m_paintFloor;
    private Task m_wanderRandomDirection;

    private WanderState m_wState = WanderState.NotWandering;
    private WanderState m_lastWState = WanderState.NotWandering;

    protected override void Start()
    {
        m_hscript = GetComponent<HealthScript>();
        m_animManager = GetComponent<AnimationManager>();
        m_navMeshAgent = GetComponent<NavMeshAgent>();

        // Override the variables in m_navMeshAgent if they're not set already.
        m_navMeshAgent.speed = m_speed;
        m_navMeshAgent.stoppingDistance = m_attackRange;

        m_paintFloor = new Task(PaintOnFloorLoop());
        m_wanderRandomDirection = new Task(ShuffleRandomDirection());
        m_wanderRandomDirection.Pause();
    }

    // Consider what the AI will do at any point, and handles AI animation
    protected override void ProcessAIIntent()
    {
        if (m_targetPlayer)
        {
            m_directionToPlayer = m_targetPlayer.position - transform.position;

            if (m_distanceFromPlayer < m_detectionRadius)
            {
                m_directionToPlayer.y = 0;

                if (m_speed > m_speedTriggerRun)
                {
                    m_animManager.ChangeState(AnimationManager.EnemyState.Run);
                }
                else
                {
                    m_animManager.ChangeState(AnimationManager.EnemyState.Walk);
                }
            }
            else
            {
                // wander so we can paint randomly around us
                if (m_wState == WanderState.Wander)
                {
                    if (m_speed > m_speedTriggerRun)
                    {
                        m_animManager.ChangeState(AnimationManager.EnemyState.Run);
                    }
                    else
                    {
                        m_animManager.ChangeState(AnimationManager.EnemyState.Walk);
                    }
                }
                else
                {
                    m_animManager.ChangeState(AnimationManager.EnemyState.Idle);
                }
            }
        }
        else
        {
            m_animManager.ChangeState(AnimationManager.EnemyState.Idle);
        }
    }

    // Primarily used for moving and attacking
    protected override void HandleAIIntent()
    {
        if (m_targetPlayer)
        {
            m_currentState = m_animManager.GetCurrentState();

            if (Vector3.Distance(m_targetPlayer.position, transform.position) < m_detectionRadius)
            {
                // Go the opposite direction of the player
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_directionToPlayer) * Quaternion.Euler(0, 180f, 0), 0.1f);
                if (m_wState == WanderState.Wander || m_wState == WanderState.Idle)
                {
                    m_navMeshAgent.isStopped = true;
                    m_wanderRandomDirection.Pause();
                    m_lastWState = m_wState;
                    m_wState = WanderState.NotWandering;
                }
            }
            else
            {
                // All players are far away, engage in random wanderer movement
                if (m_wState == WanderState.NotWandering)
                {
                    if (m_lastWState != WanderState.NotWandering)
                    {
                        m_wState = m_lastWState;
                    }
                    m_navMeshAgent.isStopped = false;
                    m_wanderRandomDirection.Unpause();
                }
            }

            if (IsMoving() && m_navMeshAgent.isStopped)
            {
                m_navMeshAgent.Move(transform.forward * m_speed * Time.deltaTime);
            }
        }
    }

    private IEnumerator PaintOnFloorLoop()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(m_paintCooldown);

            if (this && IsMoving())
            {
                //Debug.DrawRay(transform.position, Vector3.down * m_raycastFloor, Color.green);
                if (Physics.Raycast(transform.position, -transform.up, out m_raycastHit, m_raycastFloor, m_paintableMask))
                {
                    //the ray collided with something, you can interact
                    // with the hit object now by using hit.collider.gameObject
                    Vector3 paintPosition = new Vector3(transform.position.x, m_raycastHit.collider.gameObject.transform.position.y, transform.position.z);

                    if (!m_isUnpainter)
                    {
                        PaintingManager.PaintSphere(m_colorToPaint, paintPosition, m_paintRadius);
                    }
                    else
                    {
                        PaintingManager.UnpaintSphere(paintPosition, m_paintRadius);
                    }
                }
            }
        }
    }

    private IEnumerator ShuffleRandomDirection()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(Random.Range(m_wanderTime.minTime, m_wanderTime.maxTime));

            m_navMeshAgent.SetDestination(GetRandomPosition()); // choose random direction

            m_wState = WanderState.Wander;
            yield return new WaitForSecondsRealtime(Random.Range(m_idleTime.minTime, m_idleTime.maxTime));

            m_wState = WanderState.Idle;
            m_navMeshAgent.SetDestination(transform.position); // set destination to current destination so it wont keep moving
        }
    }

    // Find a random position on the map that the NavMeshAgent can travel to
    private Vector3 GetRandomPosition()
    {
        //float randomZ;
        //float randomX;
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

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
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

    private bool IsMoving()
    {
        return m_currentState == AnimationManager.EnemyState.Walk || m_currentState == AnimationManager.EnemyState.Run;
    }
}
