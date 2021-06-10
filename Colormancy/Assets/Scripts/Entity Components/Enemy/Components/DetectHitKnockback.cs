using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class DetectHitKnockback : DetectHit
{
    // Descendant of DetectHit, does everything it does, but also applies
    // a knockback (originating from center of hitbox)
    // These forces will only be applied to players.

    #region Private variables

    [System.Serializable]
    private enum ForceType
    {
        None,
        Knockback,
        Suck,
        Push
    }
    
    [SerializeField]
    private bool m_onlyDoDamageOnce = false; // is this like an explosion attack?

    [SerializeField]
    private bool m_applyForce = false;

    [MyBox.ConditionalField("m_applyForce", false)]
    [SerializeField]
    private ForceType m_whatForce = ForceType.None;

    [MyBox.ConditionalField("m_whatForce", true, ForceType.None)]
    [SerializeField]
    private float m_force = 150f;

    private List<int> m_playersHurt = new List<int>();

    #endregion

    #region Private functions

    /// <summary>
    /// This checks if the player has been slammed with the attack already (presuming that m_onlyDoDamageOnce is true)
    /// </summary>
    /// <param name="playerID">The photon player Actor ID.</param>
    /// <returns>Whether this can hurt you or not</returns>
    private bool CanThisHurtYou(int playerID)
    {
        if (m_playersHurt.FindIndex(x => x == playerID) < 0)
        {
            m_playersHurt.Add(playerID);
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #region Overrided functions

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && m_applyForce && m_whatForce != ForceType.None)
        {
            StatusEffectScript statEffectScript = other.gameObject.GetComponent<StatusEffectScript>();
            if (m_whatForce == ForceType.Knockback && other.gameObject)
            {
                statEffectScript.RPCApplyForce(Time.deltaTime * 1.5f, "Knockback", (other.gameObject.transform.position - transform.position).normalized,
                                                m_force);
            }
            else if (m_whatForce == ForceType.Suck && other.gameObject)
            {
                statEffectScript.RPCApplyForce(Time.deltaTime * 1.5f, "Suck", (transform.position - other.gameObject.transform.position).normalized, m_force);
            }
            else if (m_whatForce == ForceType.Push && other.gameObject)
            {
                statEffectScript.RPCApplyForce(Time.deltaTime * 1.5f, "Push", transform.forward, m_force);
            }
        }

        if (!m_onlyDoDamageOnce)
        {
            base.OnTriggerEnter(other);
        }
    }

    protected override void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && m_applyForce && m_whatForce != ForceType.None)
        {
            StatusEffectScript statEffectScript = other.gameObject.GetComponent<StatusEffectScript>();
            if (m_whatForce == ForceType.Knockback && other.gameObject)
            {
                statEffectScript.RPCApplyForce(Time.deltaTime * 1.5f, "Knockback", (other.gameObject.transform.position - transform.position).normalized,
                                                m_force * Time.deltaTime);
            }
            else if (m_whatForce == ForceType.Suck && other.gameObject)
            {
                statEffectScript.RPCApplyForce(Time.deltaTime * 1.5f, "Suck", (transform.position - other.gameObject.transform.position).normalized, m_force * Time.deltaTime);
            }
            else if (m_whatForce == ForceType.Push && other.gameObject)
            {
                statEffectScript.RPCApplyForce(Time.deltaTime * 1.5f, "Push", transform.forward, m_force * Time.deltaTime);
            }
        }

        if (m_onlyDoDamageOnce)
        {
            // Apparently if you're not the masterclient, and you get bombed, OnTriggerEnter basically
            // never gets called, so this is implemented so that players will still get hurt
            // by bombs once
            if (other.CompareTag("Player"))
            {
                PhotonView playerPhotonView = PhotonView.Get(other.gameObject);
                if (playerPhotonView && playerPhotonView.IsMine)
                {
                    int playerID = playerPhotonView.Controller.ActorNumber;

                    if (m_isProjectile)
                    {
                        if (CanThisHurtYou(playerID))
                        {
                            playerPhotonView.RPC("TakeDamage", playerPhotonView.Owner, m_damage * m_damageMultiplier);
                        }
                    }
                    else if (m_parentHurtboxScript && m_parentHurtboxScript.IsPlayerValidTarget(playerPhotonView.ViewID))
                    {
                        if (CanThisHurtYou(playerID))
                        {
                            m_parentHurtboxScript.RPCInsertHurtVictim(playerPhotonView.ViewID);
                            playerPhotonView.RPC("TakeDamage", playerPhotonView.Owner, m_damage * m_damageMultiplier);
                        }
                    }
                }
            }
        }
        else
        {
            base.OnTriggerStay(other);
        }
    }

    #endregion

}
