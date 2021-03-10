using UnityEngine;
using UnityEngine.UI;

public class PaintProgressUI : MonoBehaviour
{
    // Updates the paint progress based on PaintingManager.paintingProgress();

    #region Private variables

    [SerializeField]
    private Image m_paintFillBar;

    private GameManager m_gmScript;

    #endregion

    #region MonoBehaviour callbacks

    private void Start()
    {
        m_gmScript = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (!m_gmScript.IsLevel)
        {
            // disable this gameObject for nonlevel scenes
            gameObject.SetActive(false);
        }
    }


    // Update is called once per frame
    private void Update()
    {
        m_paintFillBar.fillAmount = PaintingManager.paintingProgress() / m_gmScript.PaintPercentageNeededToWin;
    }

    #endregion
}
