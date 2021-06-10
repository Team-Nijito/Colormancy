using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
[DisallowMultipleComponent]
public class EnemyPaintAbility : MonoBehaviour
{
    // This script allows an enemy to paint (or unpaint) a trail under them as they move

    #region Accessors (c# Properties)

    public Color ColorToPaint { get { return m_colorToPaint; } protected set { m_colorToPaint = value; } }
    public bool IsUnpainter { get { return m_isUnpainter; } protected set { m_isUnpainter = value; } }
    public float PaintCooldown { get { return m_paintCooldown; } protected set { m_paintCooldown = value; } }
    public float PaintRadius { get { return m_paintRadius; } protected set { m_paintRadius = value; } }
    public float RaycastFloorLength { get { return m_raycastFloorLen; } protected set { m_raycastFloorLen = value; } }

    #endregion

    #region Variables

    // Painting settings
    [SerializeField]
    protected Color m_colorToPaint = Color.red;

    [SerializeField]
    protected bool m_isUnpainter = true; // normally unpaints instead of paint

    [SerializeField]
    protected float m_paintCooldown = 0.5f; // interval between "paints"

    [SerializeField]
    protected float m_paintRadius = 3f;

    [SerializeField]
    protected float m_raycastFloorLen = 2f; // how large is the laser we shoot downwards to check for a ground

    [SerializeField]
    protected LayerMask m_paintableMask; // only focus on the paintable mask

    protected RaycastHit m_raycastHit;
    protected Task m_paintFloor;

    #endregion

    #region Components

    private EnemyMovement m_enemMovement;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        m_enemMovement = GetComponent<EnemyMovement>();
        m_paintableMask = LayerMask.GetMask("Paintable");
        m_paintFloor = new Task(PaintOnFloorLoop());
    }

    #region Protected functions

    /// <summary>
    /// A function to paint a trail on the floor as the enemy moves around. Can paint/unpaint while idling.
    /// Rinse and repeat.
    /// </summary>
    protected IEnumerator PaintOnFloorLoop()
    {
        while (true)
        {
            if (this)
            {
                // by allowing the painter skeleton to paint while idling, we allowed other clients to witness the painting
                // because the enemies they see are simply synced by position, and not being moved by the movement scripts
                // tl;dr isMoving will always be false for non master clients

                if (m_isUnpainter)
                {
                    PaintingManager.UnpaintSphere(transform.position, m_paintRadius);
                }
                else
                {
                    PaintingManager.PaintSphere(m_colorToPaint, transform.position, m_paintRadius);
                }
            }
            yield return new WaitForSecondsRealtime(m_paintCooldown);
        }
    }

    #endregion

    #region Public functions

    /// <summary>
    /// Is the AI currently painting?
    /// </summary>
    public bool IsCurrentlyPainting()
    {
        return m_paintFloor.Running;
    }

    /// <summary>
    /// Pause the painting process.
    /// </summary>
    public void PausePainting()
    {
        m_paintFloor.Pause();
    }

    /// <summary>
    /// Stop all ongoing Tasks or coroutines.
    /// </summary>
    public void StopAllTasks()
    {
        if (m_paintFloor != null)
        {
            m_paintFloor.Stop();
        }
    }

    /// <summary>
    /// Unpause the painting process.
    /// </summary>
    public void UnpausePainting()
    {
        m_paintFloor.Unpause();
    }

    #endregion
}
