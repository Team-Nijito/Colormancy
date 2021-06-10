using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Force : StatusEffect
{
    /// <summary>
    /// The force status effect will afflict a force upon all entities.
    /// 
    /// Force has now changed (for AI entities) in the sense that it will utilize NavMeshAgent's warp (thanks Spring!) instead of the 
    /// rigidbody physics functions. I've felt that the entities are slightly more resilient to forces compared to players, 
    /// so I've added a force multiplier for the AI entities.
    /// 
    /// Only one instance of this class would exist in the list of status effects at a time.
    /// (any additional forces can be appended to the additional force vector)
    /// </summary>

    #region Accessors (c# Properties)

    public Vector3 ForceVector { get { return m_impact; } protected set { m_impact = value; } }

    #endregion

    #region Variables

    protected bool m_isPlayer;
    protected Vector3 m_impact;

    #endregion

    #region Player components

    // For now these are static so that this force will act the same for all players
    // we could make these an actual parameter for our constructor, but we have enough
    // parameters on there.
    [SerializeField] protected static readonly float m_characterMass = 1f;
    [SerializeField] protected static readonly float m_impactDecay = 5f; // how quickly impact "goes away"

    protected CharacterController m_controller;

    #endregion

    #region AI components

    protected Transform m_enemTransform;
    protected EnemyMovement m_enemMove;

    #endregion

    #region Functions

    public Force(List<StatusEffect> parentList, StatusType type, float duration, string source, Vector3 direction, 
                 float forcePower, CharacterController playerController)
                 : base(parentList, type, duration, source)
    {
        // force upon player constructor
        m_isPlayer = true;
        m_controller = playerController;

        // initialize force
        m_impact = Vector3.zero;
        AppendForce(direction, forcePower);
    }

    public Force(List<StatusEffect> parentList, StatusType type, float duration, string source, Vector3 direction, 
                 float forcePower, EnemyMovement enemMoveScript, Transform enemTransform)
                 : base(parentList, type, duration, source)
    {
        // force upon AI entity constructor
        m_isPlayer = false;
        m_enemTransform = enemTransform;
        m_enemMove = enemMoveScript;

        // disable NavMeshAgent (for force) and enable rigidbody (for gravity) during the force duration
        m_enemMove.DisableAgent(true);

        // initialize force
        m_impact = Vector3.zero;
        AppendForce(direction, forcePower);
    }

    /// <summary>
    /// Normalize and append the force to m_impact. 
    /// </summary>
    public void AppendForce(Vector3 direction, float forcePower)
    {
        direction.Normalize();
        if (direction.y < 0)
        {
            direction.y = -direction.y; // reflect down force on the ground
        }

        if (m_isPlayer)
        {
            m_impact += direction * forcePower / m_characterMass;
        }
        else
        {
            m_impact += direction * forcePower / m_enemMove.Mass;
        }
    }

    public override void DoStatusEffect()
    {
        if (m_isPlayer)
        {
            // apply the impact force:
            if (m_impact.magnitude > 0.2)
            {
                m_controller.Move(m_impact * Time.deltaTime);
            }
        }
        else
        {
            // Using warp instead of using rigidbody to enact force
            //m_enemNavAgent.Warp(m_enemTransform.position + m_impact * Time.deltaTime);

            // Found that the NavMeshAgent reacts violently (i.e. teleporting up and down) when
            // you warp the NavMeshAgent into the air, I found that it's better to set the position
            // manually and just enable rigidbody (for gravity) 
            if (m_impact.magnitude > 0.2)
            {
                m_enemTransform.position += m_impact * Time.deltaTime;
            }
            else
            {
                // fall faster, otherwise the character would be "floaty"
                m_enemMove.RigidbodyAddForce(Physics.gravity * 0.5f, ForceMode.Impulse);
            }
        }

        // consumes the impact energy each cycle:
        m_impact = Vector3.Lerp(m_impact, Vector3.zero, m_impactDecay * Time.deltaTime);
    }

    /// <summary>
    /// Remove this StatusEffect from a list of status effects, and renable the NavMeshAgent.
    /// </summary>
    public override void Stop()
    {
        if (m_parentList == null) return;

        if (!m_isPlayer)
        {
            m_enemMove.EnableAgent(true);
        }

        m_parentList.Remove(this);
    }

    #endregion
}
