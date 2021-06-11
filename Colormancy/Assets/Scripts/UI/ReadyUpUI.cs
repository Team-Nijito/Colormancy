using Photon.Pun;
using System.Collections;
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

    [SerializeField]
    private bool m_doWeCareThatYouHaveOrbs = true;

    private bool m_isPlayerReady = false;

    // just store these two variables so we don't have to constantly update display text
    private int m_currentPlayersReady = -1;
    private int m_numberPlayersInRoom = -1;

    private float m_displayNeedOrbMessageSeconds = 3f;
    private float m_oldButtonTextSize;

    private Coroutine m_currentDisplayTextCouroutine = null;

    #endregion

    #region MonoBehaviour callbacks

    private void Start()
    {
        m_gmScript = GameObject.Find("GameManager").GetComponent<GameManager>();
        m_oldButtonTextSize = m_readyButtonText.fontSize;

        if (m_gmScript.TypeOfLevel == GameManager.LevelTypes.Level || m_gmScript.TypeOfLevel == GameManager.LevelTypes.BossLevel)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            if (m_currentPlayersReady != m_gmScript.PlayersReady || m_numberPlayersInRoom != PhotonNetwork.CurrentRoom.PlayerCount)
            {
                m_currentPlayersReady = m_gmScript.PlayersReady;
                m_numberPlayersInRoom = PhotonNetwork.CurrentRoom.PlayerCount;

                m_readyStateText.text = $"{m_gmScript.PlayersReady}/{m_numberPlayersInRoom} ready";
            }
        }
    }

    #endregion

    #region Private functions

    private IEnumerator TellPlayerTheyNeedAtleastOneOrb()
    {
        PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GameManager.OrbsNeededKey, out object num);
        m_readyButtonText.text = "You need at least " + (int)num + " orbs!";
        m_readyButtonText.fontSize = 12f; // magic small number that allows the text to be displayed

        yield return new WaitForSecondsRealtime(m_displayNeedOrbMessageSeconds);

        // revert to normal
        m_readyButtonText.text = "Ready up";
        m_readyButtonText.fontSize = m_oldButtonTextSize;
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
            if (m_doWeCareThatYouHaveOrbs)
            {
                PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GameManager.OrbsNeededKey, out object num);
                if (OrbManager.orbHistory.Count > ((int)num -1))
                {
                    m_readyButtonText.text = "Ready!";
                    m_readyButtonImg.color = Color.green;
                    m_gmScript.RPCReadyUp();
                    m_isPlayerReady = true;
                }
                else
                {
                    if (m_currentDisplayTextCouroutine != null)
                    {
                        // if the user spams the button, punish the user by restarting the countdown
                        StopCoroutine(m_currentDisplayTextCouroutine);
                    }
                    m_currentDisplayTextCouroutine = StartCoroutine(TellPlayerTheyNeedAtleastOneOrb());
                }
            }
            else
            {
                // don't care abouts orbs, this is a generic ready up system (anyone can ready up at any time they want)
                m_readyButtonText.text = "Ready!";
                m_readyButtonImg.color = Color.green;
                m_gmScript.RPCReadyUp();
                m_isPlayerReady = true;
            }
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
