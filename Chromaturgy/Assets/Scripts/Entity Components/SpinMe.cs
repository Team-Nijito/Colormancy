using UnityEngine;

public class SpinMe : MonoBehaviour
{
    // spins an object continously in any direction

    #region Private variables

    [SerializeField]
    [Range(-1.0f, 1.0f)]
    private float m_xForceDirection = 0.0f;
    [SerializeField]
    [Range(-1.0f, 1.0f)]
    private float m_yForceDirection = 0.0f;
    [SerializeField]
    [Range(-1.0f, 1.0f)]
    private float m_zForceDirection = 0.0f;
    [SerializeField]
    private float m_speedMultiplier = 1;
    [SerializeField]
    private bool m_worldPivot = false;

    private Space m_spacePivot = Space.Self;

    #endregion

    #region MonoBehaviour callbacks

    void Start()
    {
        if (m_worldPivot)
        {
            m_spacePivot = Space.World;
        }
    }

    void Update()
    {
        transform.Rotate(m_xForceDirection * m_speedMultiplier, m_yForceDirection * m_speedMultiplier, m_zForceDirection * m_speedMultiplier, m_spacePivot);
    }

    #endregion
}
