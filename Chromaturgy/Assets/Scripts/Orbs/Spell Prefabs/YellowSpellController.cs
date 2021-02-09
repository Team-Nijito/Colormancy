using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowSpellController : MonoBehaviour
{
    [SerializeField]
    public Transform playerTransform;

    [SerializeField]
    private AnimationCurve rotationScale;
    [SerializeField]
    private AnimationCurve positionScale;

    [SerializeField]
    private float rotationSpeed;
    private float startTime;
    [SerializeField]
    private float lifetime;
   
    private Vector3 fromPlayer;

    [SerializeField]
    private float spherePaintRadius;
    [SerializeField]
    private Color paintColor;

    [SerializeField]
    private bool debug;

    private int tick;

    void OnEnable()
    {
        startTime = Time.time;

        rotationScale.postWrapMode = WrapMode.Loop;
        positionScale.postWrapMode = WrapMode.Loop;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        tick++;

        if (Time.time - startTime > lifetime && !debug)
            Destroy(gameObject);

        for (int i = 0; i < transform.childCount; i++) {
            // save the new transformation
            fromPlayer = transform.GetChild(i).position - transform.position;

            // get correct distance and vector from player first
            transform.GetChild(i).position = transform.position + fromPlayer.normalized * positionScale.Evaluate((Time.time - startTime) / lifetime);

            // then rotate
            transform.GetChild(i).RotateAround(transform.position, Vector3.up, rotationScale.Evaluate((Time.time - startTime) / lifetime) * rotationSpeed / fromPlayer.magnitude);

            if (tick == (PaintingManager.paintingTickFrequency - i) % PaintingManager.paintingTickFrequency + 1)
                PaintingManager.PaintSphere(paintColor, transform.GetChild(i).position, spherePaintRadius);
        }

        if (tick == PaintingManager.paintingTickFrequency)
            tick = 0;
    }
}
