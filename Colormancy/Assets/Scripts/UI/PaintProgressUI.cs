using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PaintProgressUI : MonoBehaviour
{
    // Updates the paint progress based on PaintingManager.paintingProgress();

    #region Private variables

    [SerializeField]
    private Image m_paintFillBar;

    [SerializeField]
    private RectTransform m_brush;

    private readonly float m_paintFillGUIPercentWidthScreen = 0.416f;
    private readonly float m_brushInitialXPercentWidthBar = 0.25f; // percentage of bar width from left to right
    private readonly float m_brushEndXPercentWidthBar = 0.9583f; // percentage of bar width from left to right

    private float m_brushInitialXPosition;
    private float m_brushEndXPosition;
    private float m_brushTravelDistance;

    private GameManager m_gmScript;

    #endregion

    #region MonoBehaviour callbacks

    private void Start()
    {
        m_gmScript = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (!(m_gmScript.TypeOfLevel == GameManager.LevelTypes.Level))
        {
            // destroy this gameObject for nonlevel scenes
            Destroy(gameObject);
        }

        // paint brush should be placed relative to the position of the PaintBar meter
        m_brushInitialXPosition = Screen.width * m_paintFillGUIPercentWidthScreen * m_brushInitialXPercentWidthBar;
        m_brushEndXPosition = Screen.width * m_paintFillGUIPercentWidthScreen * m_brushEndXPercentWidthBar;

        m_brushTravelDistance = m_brushEndXPosition - m_brushInitialXPosition;
    }

    // Update is called once per frame
    private void Update()
    {
        m_paintFillBar.fillAmount = m_gmScript.CurrentPaintPercentage / m_gmScript.PaintPercentageNeededToWin;
        m_brush.position = new Vector2(m_brushInitialXPosition + m_brushTravelDistance * m_paintFillBar.fillAmount, m_brush.position.y);
    }

    #endregion
}
