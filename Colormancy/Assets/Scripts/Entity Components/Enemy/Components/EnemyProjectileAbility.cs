using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class EnemyProjectileAbility : MonoBehaviourPun
{
    // Class for managing enemy shooting projectiles

    #region Variables
    
    [SerializeField]
    protected GameObject m_projectile;

    [SerializeField]
    protected Transform m_projectileSpawnpoint;

    //[SerializeField]
    //protected float m_spawnForward = 1f;

    //[SerializeField]
    //protected float m_spawnHeight = 1f;

    [SerializeField]
    protected float m_initialXVelocity = 15f;

    [SerializeField]
    protected float m_initialYVelocity = 2f;

    [SerializeField]
    protected float m_projectileDecay = 1.5f;

    #endregion

    #region Protected functions

    /// <summary>
    /// (PunRPC) Enemy spawns a projectile to attack a target.
    /// </summary>
    /// <param name="targetPosition">Position of the target.</param>
    /// <param name="targetDistance">How far away is the target.</param>
    [PunRPC]
    protected void SpawnProjectile()
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

    #endregion

    #region Public functions

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

    #endregion
}
