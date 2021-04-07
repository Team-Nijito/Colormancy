using Photon.Pun;
using UnityEngine;

public class TestStatusEffect : MonoBehaviour
{
    // This is a TestScript that utilizes the status effect system
    // Depending on the parameters, an entity (player or AI) would collide
    // with the GameObject that this script is parented to and will become
    // afflicted with the status effect

    #region Damage over time

    [SerializeField]
    private bool m_applyDamageOverTime = false;

    [SerializeField]
    private string m_damageType = "Poison";

    [SerializeField]
    private bool m_isPercentageDamage = true; // if true, will inflict damage dependent on victim's health

    [SerializeField]
    private float m_damage = 4f;

    [SerializeField]
    private float m_secondDuration = 5f;

    #endregion

    #region Knockback

    [SerializeField]
    private bool m_applyKnockback = false;

    [SerializeField]
    private float m_force = 150f;

    #endregion

    #region Slowdown

    [SerializeField]
    private bool m_applySlowdown = false;

    [SerializeField]
    private float m_slowDownDuration = 3f;

    [Range(0,100)]
    [SerializeField]
    private float m_percentReductionInSpeed = 50f;

    #endregion

    #region Stun

    [SerializeField]
    private bool m_applyStun = false;

    [SerializeField]
    private float m_stunDuration = 1.5f;

    #endregion

    #region Blind

    [SerializeField]
    private bool m_applyBlind = false;

    [SerializeField]
    private float m_blindDuration = 5f;

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        bool isPlayer = other.CompareTag("Player");
        bool isEnemy = other.gameObject.layer == LayerMask.NameToLayer("Enemy");
        StatusEffectScript statEffectScript;

        if (isPlayer || isEnemy)
        {
            statEffectScript = other.gameObject.GetComponent<StatusEffectScript>();

            if (PhotonNetwork.InRoom)
            {
                // online - we launched from the launcher and then joined the room
                if (m_applyDamageOverTime)
                {
                    statEffectScript.RPCApplyOrStackDoT(m_isPercentageDamage, m_damage, m_secondDuration, m_damageType);
                }
                if (m_applyKnockback)
                {
                    if (other.gameObject)
                    {
                        statEffectScript.RPCApplyForce("Knockback", Time.deltaTime, other.gameObject.transform.position - transform.position + Vector3.up, 
                                                       m_force, other.gameObject.transform.position);
                    }
                }
                if (m_applySlowdown)
                {
                    statEffectScript.RPCApplySlowdown("Slow", m_percentReductionInSpeed, m_slowDownDuration);
                }
                if (m_applyStun)
                {
                    statEffectScript.RPCApplyStun(m_stunDuration);
                    Destroy(gameObject); // destroy the gameobject to prevent consecutive stuns on collision
                }
                if (m_applyBlind)
                {
                    statEffectScript.RPCApplyBlind(m_blindDuration);
                    Destroy(gameObject); // destroy the gameobject to prevent consecutive blinds on collision
                }
            }
            else
            {
                // offline testing (you've clicked play on the current scene that's not the launcher) is currently not supported
            }
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        bool isPlayer = other.CompareTag("Player");
        bool isEnemy = other.gameObject.layer == LayerMask.NameToLayer("Enemy");
        StatusEffectScript statEffectScript;

        if (isPlayer || isEnemy)
        {
            statEffectScript = other.gameObject.GetComponent<StatusEffectScript>();

            if (PhotonNetwork.InRoom)
            {
                if (m_applyDamageOverTime)
                {
                    statEffectScript.RPCApplyOrStackDoT(m_isPercentageDamage, m_damage, m_secondDuration * Time.deltaTime, m_damageType);
                }
                if (m_applyKnockback)
                {
                    if (other.gameObject)
                    {
                        statEffectScript.RPCApplyForce("Knockback", Time.deltaTime, other.gameObject.transform.position - transform.position + Vector3.up, 
                                                       m_force * Time.deltaTime, other.gameObject.transform.position);
                    }
                }
            }
            else
            {
                // offline testing (you've clicked play on the current scene that's not the launcher) is currently not supported
            }
        }
    }
}
