using UnityEngine;
using UnityEngine.UI;

public class ScrollingBackground : MonoBehaviour
{   
    // scrolling background menu

    private Vector3 m_startPosition;

    [SerializeField]
    private Image m_imageBackground;

    [SerializeField]
    private int m_howManyImagesInBackground = 2;

    [SerializeField]
    private float m_stopDistanceOffset = 0f; 

    [SerializeField]
    private float m_distancePerSec = 1f;

    private float m_distance = 0f;
    private float m_stoppingDistance = 0f;

    private void Start()
    {
        m_startPosition = transform.position;
        if (m_imageBackground && m_howManyImagesInBackground > 1)
        {
            // assuming that there are at least 2 background images
            // and the 1st image is currently centered
            // you would have to "travel" (image len * number of images) / 2 distance
            m_stoppingDistance = m_howManyImagesInBackground * 0.5f * m_imageBackground.sprite.rect.width;
        }
        m_stoppingDistance += m_stopDistanceOffset; // nudge the stopping distances since the algorithm isn't that accurate
    }

    private void Update()
    {
        transform.position -= new Vector3(m_distancePerSec * Time.deltaTime, 0, 0);
        m_distance += m_distancePerSec * Time.deltaTime;

        if (m_distance > m_stoppingDistance)
        {
            float overflow = m_distance - m_stoppingDistance;
            m_distance = 0;
            m_distance += overflow;
            transform.position = m_startPosition - new Vector3(overflow, 0, 0);
        }
    }
}
