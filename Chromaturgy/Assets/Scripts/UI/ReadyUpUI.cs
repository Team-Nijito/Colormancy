using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReadyUpUI : MonoBehaviour
{
    #region Private variables    

    private GameManager m_gmScript;

    [SerializeField]
    private TextMeshProUGUI m_readyStateText;

    private GameObject m_readyButton;
    [SerializeField]
    private Image m_readyButtonImg;
    [SerializeField]
    private TextMeshProUGUI m_readyButtonText;

    private bool m_isPlayerReady = false;

    private int m_currentPlayersReady = -1; // just store this so we don't have to constantly update text

    #endregion

    #region MonoBehaviour callbacks

    private void Start()
    {
        m_gmScript = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (m_gmScript.IsLevel)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (m_currentPlayersReady != m_gmScript.PlayersReady)
        {
            m_currentPlayersReady = m_gmScript.PlayersReady;
            m_readyStateText.text = $"{m_gmScript.PlayersReady}/{m_gmScript.PlayersNeededToReady} ready";
        }
    }

    #endregion

    #region Event functions
    // For when we want to fetch the click events

    /// <summary>
    /// When the ready button is clicked, check to see what state we're in, and then set the options appropriately.
    /// </summary>
    public void OnClick()
    {
        if (!m_isPlayerReady)
        {
            m_readyButtonText.text = "Ready!";
            m_readyButtonImg.color = Color.green;
            m_gmScript.RPCReadyUp();
            m_isPlayerReady = true;
        }
        else
        {
            m_readyButtonText.text = "Ready up";
            m_readyButtonImg.color = Color.red;
            m_gmScript.RPCUnready();
            m_isPlayerReady = false;
        }
    }

    #endregion
}
