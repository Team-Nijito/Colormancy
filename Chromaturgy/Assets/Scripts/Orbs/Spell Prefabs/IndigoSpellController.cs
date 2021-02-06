using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndigoSpellController : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private AnimationCurve positionScale;

    private Vector3 fromPlayer;

    private float startTime;
    [SerializeField]
    private float lifetime;

    [SerializeField]
    private float spherePaintRadius;
    [SerializeField]
    private Color paintColor;

    [SerializeField]
    private bool debug;

    // Start is called before the first frame update
    void OnEnable()
    {
        startTime = Time.time;

        PaintingManager.PaintSphere(paintColor, transform.position, spherePaintRadius, 0.8f);
    }

    void FixedUpdate()
    {
        if (Time.time - startTime > lifetime && !debug)
            Destroy(gameObject);

        for (int i = 0; i < transform.childCount; i++)
        {
            // save the new transformation
            fromPlayer = transform.GetChild(i).position - playerTransform.position;

            // get correct distance and vector from player first
            transform.GetChild(i).position = playerTransform.position + fromPlayer.normalized * positionScale.Evaluate((Time.time - startTime) / lifetime);

            // where to store globals? eg layermask
            PaintingManager.PaintSphere(paintColor, transform.GetChild(i).position, spherePaintRadius);
        }
    }
}
