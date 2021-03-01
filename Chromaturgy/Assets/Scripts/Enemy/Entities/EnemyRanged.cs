using Photon.Pun;
using UnityEngine;

public class EnemyRanged : EnemyChase
{
    // Similar to an EnemyChaser, but shoots projectiles at the players instead of meleeing

    // Note ; ranged enemies move closer if they miss
    // tried to make them reposition so they have a better chance
    // at hitting the target, but there are still bugs
    // (like the ranged enemies continuously firing... and not moving if player stay in place)
    // and the spawnpoint of the projectile being behind the skeleton

    [SerializeField]
    private GameObject m_projectile;

    [SerializeField]
    private Transform m_projectileSpawnpoint;

    //[SerializeField]
    //private float m_spawnForward = 1f;

    //[SerializeField]
    //private float m_spawnHeight = 1f;

    [SerializeField]
    private float m_initialXVelocity = 15f;

    [SerializeField]
    private float m_initialYVelocity = 2f;

    [SerializeField]
    private float m_projectileDecay = 1.5f;

    private float m_tempAttackRange;

    protected override void Start()
    {
        m_tempAttackRange = m_attackRange;
        base.Start();
    }

    /// <summary>
    /// Consider what the AI will do at any point, and handles AI animation
    /// </summary>
    protected override void ProcessAIIntent()
    {
        if (m_targetPlayer)
        {
            m_directionToPlayer = m_targetPlayer.position - transform.position;
            m_angleFromPlayer = Vector3.Angle(m_directionToPlayer, transform.forward);

            if (m_distanceFromPlayer < m_closeDetectionRadius)
            {
                // player got too close, they're detected
                // raycast here to see if there is an object between AI and player
                if (canSeePlayer())
                {
                    photonView.RPC("PlayerIsDetected", RpcTarget.All);
                }
            }
            else if (m_distanceFromPlayer < m_detectionRadius && m_angleFromPlayer < m_fieldOfView)
            {
                // player is in cone vision
                // raycast here to see if there is an object between AI and player
                if (canSeePlayer())
                {
                    photonView.RPC("PlayerIsDetected", RpcTarget.All);
                }
            }
            else
            {
                if (m_rememberTarget)
                {
                    // still remember target, go after them
                    photonView.RPC("StartForgettingTask", RpcTarget.All);
                }

                if (m_rememberTarget || m_isForgettingTarget)
                {
                    // start forgetting the target, but still target them
                    // until AI completely forgets target
                    photonView.RPC("PlayerIsTargetted", RpcTarget.All);
                }
                else
                {
                    // don't see player, just idle for now
                    m_animManager.ChangeState(AnimationManager.EnemyState.Idle);
                }
            }
        }
        else
        {
            m_animManager.ChangeState(AnimationManager.EnemyState.Idle);
        }
    }

    /// <summary>
    /// (PunRPC) This is invoked in ProcessAIIntent whenever a player is considered "remembered" instead of "detected"
    /// and is also invoked by PlayerIsDetected()
    /// </summary>
    [PunRPC]
    protected override void PlayerIsTargetted()
    {
        if (m_directionToPlayer.magnitude > m_tempAttackRange)
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
            m_animManager.ChangeState(AnimationManager.EnemyState.Attack);
        }
    }

    /// <summary>
    /// (PunRPC) Enemy spawns a projectile to attack a target.
    /// </summary>
    /// <param name="targetPosition">Position of the target.</param>
    /// <param name="targetDistance">How far away is the target.</param>
    [PunRPC]
    private void SpawnProjectile()
    {
        Vector3 spawnPosition = m_projectileSpawnpoint.position;
        GameObject projectile = Instantiate(m_projectile, spawnPosition, m_projectileSpawnpoint.rotation);
        Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();
        projectile.GetComponent<DetectHit>().SetParentGameObject(gameObject);

        // lob projectile if the target y position is higher than ours
        projectileRB.AddForce(projectile.transform.forward * m_initialXVelocity + transform.up * m_initialYVelocity, ForceMode.Impulse);

        // angular velocity messes up trajectory
        // which is why projectile doesn't spin
        Destroy(projectile, m_projectileDecay);
    }

    /// <summary>
    /// (PunRPC) Move the AI closer so that the AI would have a better chance at hitting the target (hypothetically)
    /// </summary>
    [PunRPC]
    private void RangeGetCloser()
    {
        ChangeAttackStoppingRange(-1);
    }

    /// <summary>
    /// (PunRPC) Move the AI away so that the AI would be farther away from player
    /// </summary>
    [PunRPC]
    private void RangeGetFarther()
    {
        ChangeAttackStoppingRange(0.1f);
    }

    /// <summary>
    /// Change the attack and stopping range so that the AI would move closer to / farther away from the player.
    /// </summary>
    /// <param name="changeVal">How much to decrease the range by</param>
    private void ChangeAttackStoppingRange(float changeVal)
    {
        if (changeVal < 0)
        {
            // decrease range
            if ((m_navMeshAgent.stoppingDistance + changeVal) >= m_closeDetectionRadius)
            {
                m_tempAttackRange += changeVal;
                m_navMeshAgent.stoppingDistance += changeVal;
            }
        }
        else
        {
            // increase range
            if ((m_tempAttackRange + changeVal) < m_attackRange)
            {
                m_tempAttackRange += changeVal;
                m_navMeshAgent.stoppingDistance += changeVal;

                m_navMeshAgent.Move((transform.position - m_targetPlayer.position).normalized * changeVal);
            }
        }
    }

    /// <summary>
    /// Increase the attack and stopping range so that the AI would move away to the player.
    /// </summary>
    /// <param name="increaseVal">How much to decrease the range by</param>
    private void IncreaseAttackStoppingRange(float increaseVal)
    {
        if ((m_tempAttackRange + increaseVal) < m_attackRange)
        {
            m_tempAttackRange += increaseVal;
            m_navMeshAgent.stoppingDistance += increaseVal;

            // move agent away from player
            m_navMeshAgent.Move((transform.position - m_targetPlayer.position).normalized * increaseVal);
        }
        else
        {
            if (m_tempAttackRange != m_attackRange)
            {
                m_tempAttackRange = m_navMeshAgent.stoppingDistance = m_attackRange;
            }
        }
    }

    /// <summary>
    /// Wrapper function for spawning projectile
    /// </summary>
    public void RPCSpawnProjectile()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("SpawnProjectile", RpcTarget.All);
        }
    }
}
