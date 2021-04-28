using UnityEngine;

public class EnemyAnimationManager : MonoBehaviour
{
    // Manages an enemy's animation
    
    private Animator m_animator;
    private EnemyState m_currentState = EnemyState.Idle;
    
    public enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Death
    }

    // Animator parameter hashes
    public readonly int speedHash = Animator.StringToHash("Speed");
    public readonly int isMovingAnimHash = Animator.StringToHash("IsMoving");
    public readonly int isAttackingAnimHash = Animator.StringToHash("IsAttacking");
    public readonly int isDyingAnimHash = Animator.StringToHash("IsDying");

    private void Start()
    {
        m_animator = GetComponent<Animator>();

        if (m_animator)
        {
            m_animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
        }
    }

    /// <summary>
    /// Changes the enemy's current state and changes its animation accordingly
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(EnemyState newState)
    {
        // stop the same animation from interupting itself
        if (m_currentState == newState) return;

        switch (newState)
        {
            case EnemyState.Idle:
                m_animator.SetBool(isAttackingAnimHash, false);
                m_animator.SetBool(isMovingAnimHash, false);
                break;
            case EnemyState.Move:
                m_animator.SetBool(isAttackingAnimHash, false);
                m_animator.SetBool(isMovingAnimHash, true);
                break;
            case EnemyState.Attack:
                m_animator.SetBool(isAttackingAnimHash, true);
                m_animator.SetBool(isMovingAnimHash, false);
                break;
            case EnemyState.Death:
                m_animator.SetBool(isDyingAnimHash, true);
                m_animator.SetBool(isAttackingAnimHash, false);
                m_animator.SetBool(isMovingAnimHash, false);
                break;
        }
        m_currentState = newState;
    }

    public EnemyState GetCurrentState()
    {
        return m_currentState;
    }

    /// <summary>
    /// Set the entity movement animation's speed.
    /// </summary>
    /// <param name="newSpeed"></param>
    public void SetSpeed(float newSpeed)
    {
        m_animator.SetFloat(speedHash, newSpeed);
    }
}
