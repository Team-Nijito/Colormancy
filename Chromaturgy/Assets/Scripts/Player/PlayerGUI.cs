using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerGUI : MonoBehaviour
{
    public Image m_healthBar;
    public Image m_ManaBar;

    private GameObject m_playerTarget; // the player whose health and mana to track
    private HealthScript m_playerHealth;
    private ManaScript m_playerMana;

    void Awake()
    {
        // Set this healthbar as a child of a canvas (so it will be displayed properly) 
        transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_playerTarget)
        {
            m_healthBar.fillAmount = m_playerHealth.GetEffectiveHealth() / m_playerHealth.GetMaxEffectiveHealth();
            m_ManaBar.fillAmount = m_playerMana.GetEffectiveMana() / m_playerMana.GetMaxEffectiveMana();
        }
        else
        {
            // Destroy itself if the target is null, It's a fail safe when Photon is destroying Instances of a Player over the network
            // tl;dr if the player doesn't exist, destroy the playerGUI
            Destroy(gameObject);
            return;
        }
    }

    // just call this whenever we know for sure that m_player exists
    void AssignLocalVariables()
    {
        m_playerHealth = m_playerTarget.GetComponent<HealthScript>();
        m_playerMana = m_playerTarget.GetComponent<ManaScript>();
    }

    public void SetTarget(GameObject _target)
    {
        if (_target == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> GameObject target for PlayerGUI.SetTarget.", this);
            return;
        }
        // Cache references for efficiency
        m_playerTarget = _target;
        
        // Cpde snippet for names above players ....
        //if (playerNameText != null)
        //{
        //    playerNameText.text = target.photonView.Owner.NickName;
        //}

        if (_target != null)
        {
            AssignLocalVariables();
        }
    }
}
