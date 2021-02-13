using Photon.Pun;
using UnityEngine;

public class EnemyRanged : EnemyChase
{
    // Similar to an EnemyChaser, but shoots projectiles at the players instead of meleeing

    [SerializeField]
    private GameObject m_projectile;

    [SerializeField]
    private float m_spawnForward = 3f;

    [SerializeField]
    private float m_spawnHeight = 3f;

    [SerializeField]
    private float m_initialXVelocity = 15f;

    [SerializeField]
    private float m_initialYVelocity = 2f;

    [SerializeField]
    private float m_projectileDecay = 1.5f;

    // Consider what the AI will do at any point, and handles AI animation
    protected override void ProcessAIIntent()
    {
        if (m_targetPlayer)
        {
            m_directionToPlayer = m_targetPlayer.position - transform.position;

            if (m_distanceFromPlayer < m_detectionRadius)
            {
                m_directionToPlayer.y = 0;

                if (m_directionToPlayer.magnitude > m_attackRange)
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
            else
            {
                m_animManager.ChangeState(AnimationManager.EnemyState.Idle);
            }
        }
        else
        {
            m_animManager.ChangeState(AnimationManager.EnemyState.Idle);
        }
    }

    [PunRPC]
    private void SpawnProjectile()
    {
        Vector3 spawnPosition = transform.position + transform.forward * m_spawnForward + transform.up * m_spawnHeight;
        GameObject projectile = Instantiate(m_projectile, spawnPosition, transform.rotation);
        projectile.GetComponent<DetectHit>().SetParentGameObject(gameObject);
        projectile.GetComponent<Rigidbody>().AddForce(transform.forward * m_initialXVelocity + transform.up * m_initialYVelocity, ForceMode.Impulse);
        Destroy(projectile, m_projectileDecay);
    }

    // Wrapper function for spawning projectile
    public void RPCSpawnProjectile()
    {
        photonView.RPC("SpawnProjectile", RpcTarget.All);
    }
}
