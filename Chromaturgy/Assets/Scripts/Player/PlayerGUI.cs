using UnityEngine;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour
{
    public Image m_healthBar;
    public Image m_ManaBar;

    public GameObject m_player;
    private HealthScript m_playerHealth;
    private ManaScript m_playerMana;

    // Start is called before the first frame update
    void Start()
    {
        if (m_player)
        {
            AssignLocalVariables();
        }
        else
        {
            print($"No player assigned for PlayerGUI. Trying to look for player with tag 'player'");
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            for (short ind = 0; ind < players.Length; ind++)
            {
                // for now just take the first player and break from loop
                // if we were to networked this, we would use "Photon.ismine" to determine that this
                // is the player whose health/mana we want to track
                m_player = players[ind];
                break;
            }
            if (m_player)
            {
                print("Player assigned for PlayerGUI.");
                AssignLocalVariables();
            }
            else
                print("No player assigned for PlayerGUI.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_player)
        {
            m_healthBar.fillAmount = m_playerHealth.GetEffectiveHealth() / m_playerHealth.GetMaxEffectiveHealth();
            m_ManaBar.fillAmount = m_playerMana.GetEffectiveMana() / m_playerMana.GetMaxEffectiveMana();
        }
    }

    // just call this whenever we know for sure that m_player exists
    void AssignLocalVariables()
    {
        m_playerHealth = m_player.GetComponent<HealthScript>();
        m_playerMana = m_player.GetComponent<ManaScript>();
    }
}
