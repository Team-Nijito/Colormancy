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
    [SerializeField] protected static readonly float m_characterMass = 1.0f;
    [SerializeField] protected static readonly float m_impactDecay = 5f; // how quickly impact "goes away"

    protected CharacterController m_controller;

    #endregion

    #region AI components

    protected NavMeshAgent m_enemNavAgent;
    protected Vector3 m_enemPosition;

    // the AI entity seems to be more resilient to forces compared to the player
    // give it a nudge
    [SerializeField]  protected readonly float m_AIForceMultipier = 2f;

    #endregion

    #region Functions

    public Force(List<StatusEffect> parentList, string name, StatusType type, float duration, Vector3 direction, 
                 float forcePower, CharacterController playerController)
                 : base(parentList, name, type, duration)
    {
        // force upon player constructor
        m_isPlayer = true;
        m_controller = playerController;

        // initialize force
        m_impact = Vector3.zero;
        AppendForce(direction, forcePower);
    }

    public Force(List<StatusEffect> parentList, string name, StatusType type, float duration, Vector3 direction, 
                 float forcePower, NavMeshAgent agent, Vector3 enemPos)
                 : base(parentList, name, type, duration)
    {
        // force upon AI entity constructor
        m_isPlayer = false;
        m_enemNavAgent = agent;
        m_enemPosition = enemPos;

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

        m_impact += direction * forcePower / m_characterMass;
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
            // consumes the impact energy each cycle:
            m_impact = Vector3.Lerp(m_impact, Vector3.zero, m_impactDecay * Time.deltaTime);
        }
        else
        {
            // Using warp instead of using rigidbody to enact force
            m_enemNavAgent.Warp(m_enemPosition + m_impact * m_AIForceMultipier * Time.deltaTime);

            // consumes the impact energy each cycle:
            m_impact = Vector3.Lerp(m_impact, Vector3.zero, m_impactDecay * Time.deltaTime);
        }
    }

    #endregion
}
