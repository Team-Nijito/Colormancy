using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

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

    [SerializeField]
    private Color m_colorToPaint = Color.red;

    [SerializeField]
    private bool m_isUnpainter = true; // normally unpaints instead of paint

    [SerializeField]
    private float m_paintCooldown = 2f; // interval between "paints"

    [SerializeField]
    private float m_paintRadius = 3f;

    [SerializeField]
    private float m_raycastFloor = 1.5f;

    private RaycastHit m_raycastHit;

    private int m_paintableMask = 1 << 8; // only focus on the paintable mask

    [SerializeField]
    private RangeTime m_wanderTime;

    [SerializeField]
    private RangeTime m_idleTime;

    private Task m_paintFloor;
    private Task m_wanderRandomDirection;

    private WanderState m_wState = WanderState.NotWandering;
    private WanderState m_lastWState = WanderState.NotWandering;

    protected override void Start()
    {
        m_rbody = GetComponent<Rigidbody>();
        m_hscript = GetComponent<HealthScript>();
        m_animManager = GetComponent<AnimationManager>();

        if (m_character)
        {
            m_characterTransform = m_character.transform;
        }

        m_paintFloor = new Task(PaintOnFloorLoop());
        m_wanderRandomDirection = new Task(WanderRandomDirection());
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
                    m_wanderRandomDirection.Unpause();
                }
            }

            if (m_currentState == AnimationManager.EnemyState.Walk || m_currentState == AnimationManager.EnemyState.Run)
            {
                m_rbody.AddForce(transform.forward * m_speed);
            }
        }
    }

    private IEnumerator PaintOnFloorLoop()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(m_paintCooldown);

            if (transform)
            {
                //Debug.DrawRay(transform.position, Vector3.down * m_raycastFloor, Color.green);
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out m_raycastHit, m_raycastFloor, m_paintableMask))
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

    private IEnumerator WanderRandomDirection()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(Random.Range(m_wanderTime.minTime, m_wanderTime.maxTime));

            // choose random direction
            transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);

            m_wState = WanderState.Wander;
            yield return new WaitForSecondsRealtime(Random.Range(m_idleTime.minTime, m_idleTime.maxTime));
            m_wState = WanderState.Idle;
        }
    }
}
