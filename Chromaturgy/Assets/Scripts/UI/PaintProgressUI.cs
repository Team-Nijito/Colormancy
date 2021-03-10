using UnityEngine;
using UnityEngine.UI;

public class PaintProgressUI : MonoBehaviour
{
    // Updates the paint progress based on PaintingManager.paintingProgress();

    #region Private variables

    [SerializeField]
    private Image m_paintFillBar;

    [SerializeField]
    private RectTransform m_brush;

    [SerializeField]
    private float m_brushInitialX = 60f;

    [SerializeField]
    private float m_brushTravelLengthX = 146f; // end position is 206? idk UI scales and stuff

    private GameManager m_gmScript;

    #endregion

    #region MonoBehaviour callbacks

    private void Start()
    {
        m_gmScript = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (!m_gmScript.IsLevel)
        {
            // destroy this gameObject for nonlevel scenes
            Destroy(gameObject);
        }
    }


    // Update is called once per frame
    private void Update()
    {
        m_paintFillBar.fillAmount = PaintingManager.paintingProgress() / m_gmScript.PaintPercentageNeededToWin;
        m_brush.position = new Vector2(m_brushInitialX + m_brushTravelLengthX * m_paintFillBar.fillAmount, m_brush.position.y);
    }

    #endregion
}
