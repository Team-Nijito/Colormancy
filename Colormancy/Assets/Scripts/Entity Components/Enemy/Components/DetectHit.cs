﻿using Photon.Pun;
using UnityEngine;

public class DetectHit : MonoBehaviour
{
    // This script is used with enemy projectiles and hitboxes to do damage to players

    #region Variables

    protected enum TriggerType{
        Enter,
        Stay,
        Exit
    }

    [SerializeField]
    protected GameObject m_parentGameObject; // the parent gameobject with the PhotonView
    
    [SerializeField]
    protected bool m_isProjectile = false;

    [SerializeField]
    protected float m_damage = 12f;

    protected EnemyRangedAI m_parentERScript;
    protected EnemyHitbox m_parentHurtboxScript;
    protected PhotonView m_parentPhotonView;

    #endregion

    #region MonoBehaviour callbacks

    protected virtual void Start()
    {
        if (!m_isProjectile)
        {
            m_parentHurtboxScript = m_parentGameObject.GetComponent<EnemyHitbox>();
        }
        else
        {
            m_parentERScript = m_parentGameObject.GetComponent<EnemyRangedAI>();
        }
        m_parentPhotonView = PhotonView.Get(m_parentGameObject);
    }

    #endregion

    #region Trigger functions

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CheckApplyDamage(other, TriggerType.Enter);

            if (m_isProjectile)
            {
                Destroy(gameObject);
            }
        }
        else if (other.gameObject.layer != LayerMask.NameToLayer("Enemy") && !other.CompareTag("Projectile"))
        {    
            // Destroy a projectile if it collides with an environmental object
            if (m_isProjectile && other.name != "Zone")
            {
                Destroy(gameObject);
            }
        }
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CheckApplyDamage(other, TriggerType.Exit);

            if (m_isProjectile)
            {
                Destroy(gameObject);
            }
        }
        else if (other.gameObject.layer != LayerMask.NameToLayer("Enemy") && !other.CompareTag("Projectile"))
        {
            // Destroy a projectile if it collides with an environmental object
            if (m_isProjectile && other.name != "Zone")
            {
                Destroy(gameObject);
            }
        }
    }

    #endregion

    #region Private functions

    protected void CheckApplyDamage(Collider player, TriggerType trigType)
    {
        PhotonView playerPhotonView = PhotonView.Get(player.gameObject);
        if (playerPhotonView.IsMine)
        {
            if (m_isProjectile)
            {
                if (trigType == TriggerType.Enter)
                {
                    playerPhotonView.RPC("TakeDamage", playerPhotonView.Owner, m_damage);
                }
                else
                {
                    playerPhotonView.RPC("TakeDamage", playerPhotonView.Owner, m_damage * Time.deltaTime);
                }
            }
            else if (m_parentHurtboxScript && m_parentHurtboxScript.IsPlayerValidTarget(playerPhotonView.ViewID))
            {
                m_parentHurtboxScript.RPCInsertHurtVictim(playerPhotonView.ViewID);
                if (trigType == TriggerType.Enter)
                {
                    playerPhotonView.RPC("TakeDamage", playerPhotonView.Owner, m_damage);
                }
                else
                {
                    playerPhotonView.RPC("TakeDamage", playerPhotonView.Owner, m_damage * Time.deltaTime);
                }
            }
        }
    }

    #endregion

    #region Public functions

    public void SetParentGameObject(GameObject parent)
    {
        m_parentGameObject = parent;
    }

    #endregion
}
