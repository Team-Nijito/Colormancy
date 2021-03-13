using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndigoSpellController : MonoBehaviour
{
    [SerializeField]
    public Transform playerTransform;
    [SerializeField]
    private AnimationCurve positionScale;
    private Vector3 fromPlayer;

    [Space]

    private float startTime;
    [SerializeField]
    private float lifetime;

    [Space]

    [SerializeField]
    private float spherePaintRadius;
    [SerializeField]
    private Color paintColor;

    [SerializeField]
    private bool debug;
    private int tick;


    // Start is called before the first frame update
    void OnEnable()
    {
        startTime = Time.time;
    }

    void FixedUpdate()
    {
        tick++;

        if (Time.time - startTime > lifetime && !debug)
            Destroy(gameObject);

        for (int i = 0; i < transform.childCount; i++)
        {
            // save the new transformation
            fromPlayer = transform.GetChild(i).position - transform.position;

            // get correct distance and vector from player first
            transform.GetChild(i).position = transform.position + fromPlayer.normalized * positionScale.Evaluate((Time.time - startTime) / lifetime);

            // paint calls on separate ticks to prevent overloading of physics engine
            if (tick == (PaintingManager.paintingTickFrequency - i) % PaintingManager.paintingTickFrequency + 1)
                PaintingManager.PaintSphere(paintColor, transform.GetChild(i).position, spherePaintRadius);
        }

        if (tick == PaintingManager.paintingTickFrequency)
            tick = 0;
    }
}
