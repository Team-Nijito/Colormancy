
using UnityEngine;
public class EnemyRunStraightBombAI : EnemyChaserAI
{
    #region Private variables

    private bool m_isExploding = false; // if we're exploding, stand still, don't let any other animation play
    private bool m_disableMoveDebounce = false; // if invoking the function multiple times, only let it happen once

    #endregion

    #region Components

    private HealthScript m_hpScript;
    private EnemyMovement m_enemMove;

    #endregion

    #region Monobehaviour callbacks

    protected override void Start()
    {
        m_hpScript = GetComponent<HealthScript>();
        m_enemMove = GetComponent<EnemyMovement>();
        base.Start();
    }

    protected override void Update()
    {
        if (!m_isExploding)
        {
            //base.Update();
        }
        else
        {
            DisableMove();
        }
    }

    protected override void FixedUpdate()
    {
        if (!m_isExploding)
        {
            if (m_enemMovement.IsAgentActive() && !m_enemTargeting.IsBlind())
            {
                HandleAIIntent();
            }
        }
        else
        {
            DisableMove();
        }
    }

    protected override void HandleAIIntent()
    {
        m_enemMovement.ExitWanderingMode();

        Debug.Log("Moving to: " + (gameObject.transform.position + (gameObject.transform.forward * 2)));
        m_enemMovement.MoveToPosition(gameObject.transform.position + (gameObject.transform.forward * 2));
    }

    #endregion

    #region Public functions

    /// <summary>
    /// Prevent further movement / changes to animation,
    /// and let the current animation play out.
    /// Invoked by the animation.
    /// </summary>
    public void DisableMove()
    {
        if (!m_disableMoveDebounce)
        {
            m_disableMoveDebounce = true;
            m_isExploding = true;
            m_enemMove.MoveToPosition(transform.position); // stop moving
        }
    }

    /// <summary>
    /// Dies after exploding. Invoked by the animation.
    /// </summary>
    public void Die()
    {
        m_hpScript.ZeroHealth();
        m_animManager.SetSpeed(0f);
        m_animManager.ChangeState(EnemyAnimationManager.EnemyState.Attack);
    }

    #endregion
}