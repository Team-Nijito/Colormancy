using Photon.Pun;
using UnityEngine;
using System.Linq;

public class DetectHit : MonoBehaviour
{
    // This script is used with projectiles and hitboxes to do damage
    private enum TriggerType{
        Enter,
        Stay,
        Exit
    }

    [SerializeField]
    private GameObject m_parentGameObject; // the parent gameobject with the PhotonView
    private EnemyChase m_parentECScript;

    [SerializeField]
    private bool m_isProjectile = false;

    [SerializeField]
    private float m_damage = 12f;

    private void Start()
    {
        if (!m_isProjectile)
        {
            m_parentECScript = m_parentGameObject.GetComponent<EnemyChase>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CheckApplyDamage(other, TriggerType.Enter);

            if (m_isProjectile)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CheckApplyDamage(other, TriggerType.Exit);

            if (m_isProjectile)
            {
                Destroy(gameObject);
            }
        }
    }

    private void CheckApplyDamage(Collider player, TriggerType trigType)
    {
        PhotonView playerPhotonView = PhotonView.Get(player.gameObject);
        if (playerPhotonView.IsMine)
        {
            if (m_isProjectile)
            {
                if (trigType == TriggerType.Enter)
                {
                    playerPhotonView.RPC("TakeDamage", RpcTarget.All, m_damage);
                }
                else
                {
                    playerPhotonView.RPC("TakeDamage", RpcTarget.All, m_damage * Time.deltaTime);
                }
            }
            else if (m_parentECScript.IsPlayerValidTarget(playerPhotonView.ViewID))
            {
                m_parentECScript.RPCInsertHurtVictim(playerPhotonView.ViewID);
                if (trigType == TriggerType.Enter)
                {
                    playerPhotonView.RPC("TakeDamage", RpcTarget.All, m_damage);
                }
                else
                {
                    playerPhotonView.RPC("TakeDamage", RpcTarget.All, m_damage * Time.deltaTime);
                }
            }
        }
    }

    public void SetParentGameObject(GameObject parent)
    {
        m_parentGameObject = parent;
    }
}
