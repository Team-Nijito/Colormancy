using UnityEngine;

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
    private bool m_applyForce = false;

    [MyBox.ConditionalField("m_applyForce", false)]
    [SerializeField]
    private ForceType m_whatForce = ForceType.None;

    [MyBox.ConditionalField("m_whatForce", true, ForceType.None)]
    [SerializeField]
    private float m_force = 150f;

    #endregion

    #region Overrided functions

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && m_applyForce && m_whatForce != ForceType.None)
        {
            StatusEffectScript statEffectScript = other.gameObject.GetComponent<StatusEffectScript>();
            if (m_whatForce == ForceType.Knockback && other.gameObject)
            {
                statEffectScript.RPCApplyForce("Knockback", Time.deltaTime * 1.5f, (other.gameObject.transform.position - transform.position + Vector3.up).normalized,
                                                m_force);
            }
            else if (m_whatForce == ForceType.Suck && other.gameObject)
            {
                statEffectScript.RPCApplyForce("Suck", Time.deltaTime * 1.5f, (transform.position - other.gameObject.transform.position).normalized, m_force);
            }
            else if (m_whatForce == ForceType.Push && other.gameObject)
            {
                statEffectScript.RPCApplyForce("Push", Time.deltaTime * 1.5f, transform.forward, m_force);
            }
        }

        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && m_applyForce && m_whatForce != ForceType.None)
        {
            StatusEffectScript statEffectScript = other.gameObject.GetComponent<StatusEffectScript>();
            if (m_whatForce == ForceType.Knockback && other.gameObject)
            {
                statEffectScript.RPCApplyForce("Knockback", Time.deltaTime * 1.5f, (other.gameObject.transform.position - transform.position + Vector3.up).normalized,
                                                m_force * Time.deltaTime);
            }
            else if (m_whatForce == ForceType.Suck && other.gameObject)
            {
                statEffectScript.RPCApplyForce("Suck", Time.deltaTime * 1.5f, (transform.position - other.gameObject.transform.position).normalized, m_force * Time.deltaTime);
            }
            else if (m_whatForce == ForceType.Push && other.gameObject)
            {
                statEffectScript.RPCApplyForce("Push", Time.deltaTime * 1.5f, transform.forward, m_force * Time.deltaTime);
            }
        }

        base.OnTriggerStay(other);
    }

    #endregion

}
