using Photon.Pun;
using UnityEngine;

public class TestDoT : MonoBehaviour
{
    [SerializeField]
    private string m_damageType = "Poison";

    [SerializeField]
    private bool m_isPercentageDamage = true; // if true, will inflict damage dependent on victim's health

    [SerializeField]
    private float m_damage = 4f;

    [SerializeField]
    private float m_secondDuration = 5f;

    [SerializeField]
    private bool m_applyDamageWhenEnterObject = true;

    [SerializeField]
    private bool m_applyDamageWhileStayOnObject = true;

    private void OnTriggerEnter(Collider other)
    {
        if (m_applyDamageWhenEnterObject)
        {
            if (other.CompareTag("Player"))
            {
                // apply the DoT initally
                PhotonView.Get(other.gameObject).RPC("ApplyOrStackDoT", PhotonView.Get(other.gameObject).Owner, m_isPercentageDamage, m_damage, m_secondDuration, m_damageType);
            }
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (m_applyDamageWhileStayOnObject)
        {
            if (other.CompareTag("Player"))
            {
                // apply the DoT per frame, multiply m_secondDuration with Time.deltaTime
                PhotonView.Get(other.gameObject).RPC("ApplyOrStackDoT", PhotonView.Get(other.gameObject).Owner,
                m_isPercentageDamage, m_damage, m_secondDuration * Time.deltaTime, m_damageType);
            }
        }
    }
}
