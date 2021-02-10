using Photon.Pun;
using UnityEngine;

public class DetectHit : MonoBehaviour
{
    // This script is used with projectiles and hitboxes to do damage

    public EnemyChase m_enemyChaseScript;

    [SerializeField]
    private float m_damage = 12f;

    [SerializeField]
    private bool m_isProjectile = false;

    private GameObject m_currentVictim = null; // who we're currently damaging
    private HealthScript m_currentVictimHealthScript = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && (m_isProjectile))
        {
            DetermineVictim(other.gameObject);
            PhotonView photonView = PhotonView.Get(m_currentVictim);
            photonView.RPC("TakeDamage", RpcTarget.All, m_damage);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DetermineVictim(other.gameObject);
            PhotonView photonView = PhotonView.Get(m_currentVictim);
            photonView.RPC("TakeDamage", RpcTarget.All, m_damage * Time.deltaTime);
        }
    }

    // If this is a new victim, focus on the victim and then GetComponent<HealthScript> it
    private void DetermineVictim(GameObject entity)
    {
        if (entity != m_currentVictim)
        {
            m_currentVictim = entity;
            m_currentVictimHealthScript = m_currentVictim.GetComponent<HealthScript>();
        }
    }
}
