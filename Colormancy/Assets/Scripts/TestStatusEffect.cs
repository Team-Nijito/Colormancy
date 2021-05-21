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

    //[MyBox.ConditionalField("m_applyDamageOverTime", false)]
    //[SerializeField]
    //private string m_damageTypeName = "Poison";

    //[MyBox.ConditionalField("m_applyDamageOverTime", false)]
    //[SerializeField]
    //private bool m_isPercentageDamage = true; // if true, will inflict damage dependent on victim's health

    //[MyBox.ConditionalField("m_applyDamageOverTime", false)]
    //[SerializeField]
    //private float m_damage = 4f;

    //[MyBox.ConditionalField("m_applyDamageOverTime", false)]
    //[SerializeField]
    //private float m_secondDuration = 5f;

    #endregion

    #region Forces

    [System.Serializable]
    private enum ForceType
    {
        None,
        Knockback,
        Suck,
        Push
    }

    [SerializeField]
    private bool m_applyForce = false;

    [MyBox.ConditionalField("m_applyForce", false)]
    [SerializeField]
    private ForceType m_whatForce = ForceType.None;

    [MyBox.ConditionalField("m_whatForce", true, ForceType.None)]
    [SerializeField]
    private float m_force = 150f;

    #endregion

    #region Slowdown

    [SerializeField]
    private bool m_applySlowdown = false;

    //[MyBox.ConditionalField("m_applySlowdown", false)]
    //[SerializeField]
    //private float m_slowDownDuration = 3f;

    //[MyBox.ConditionalField("m_applySlowdown", false)]
    //[(0,100)]
    //[SerializeField]
    //private float m_percentReductionInSpeed = 50f;

    #endregion

    #region Stun

    [SerializeField]
    private bool m_applyStun = false;

    //[MyBox.ConditionalField("m_applyStun", false)]
    //[SerializeField]
    //private float m_stunDuration = 1.5f;

    #endregion

    #region Blind

    [SerializeField]
    private bool m_applyBlind = false;

    //[MyBox.ConditionalField("m_applyBlind", false)]
    //[SerializeField]
    //private float m_blindDuration = 5f;

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
                    //statEffectScript.RPCApplyOrStackDoT(m_isPercentageDamage, m_damage, m_secondDuration, m_damageTypeName);
                    //statEffectScript.RPCApplyStatus(StatusEffect.StatusType.DamageOverTime, m_)
                }
                if (m_applyForce)
                {
                    if (m_whatForce == ForceType.Knockback && other.gameObject)
                    {
                        statEffectScript.RPCApplyForce(Time.deltaTime * 1.5f, "Knockback", (other.gameObject.transform.position - transform.position + Vector3.up).normalized,
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
                if (m_applySlowdown)
                {
                    //statEffectScript.RPCApplySlowdown("Slow", m_percentReductionInSpeed, m_slowDownDuration);
                }
                if (m_applyStun)
                {
                    //statEffectScript.RPCApplyStun(m_stunDuration);
                    Destroy(gameObject); // destroy the gameobject to prevent consecutive stuns on collision
                }
                if (m_applyBlind)
                {
                    //statEffectScript.RPCApplyBlind(m_blindDuration);
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
                    //statEffectScript.RPCApplyOrStackDoT(m_isPercentageDamage, m_damage, m_secondDuration * Time.deltaTime, m_damageTypeName);
                }
                if (m_applyForce)
                {
                    if (m_whatForce == ForceType.Knockback && other.gameObject)
                    {
                        statEffectScript.RPCApplyForce(Time.deltaTime * 1.5f, "Knockback", (other.gameObject.transform.position - transform.position + Vector3.up).normalized,
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
            }
            else
            {
                // offline testing (you've clicked play on the current scene that's not the launcher) is currently not supported
            }
        }
    }
}
