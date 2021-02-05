using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowSpellController : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private AnimationCurve rotationScale;
    [SerializeField]
    private AnimationCurve positionScale;

    [SerializeField]
    private float rotationSpeed;
    private float startTime;

    private Vector3 fromPlayer;

    [SerializeField]
    private float spherePaintRadius;

    [SerializeField]
    private Color paintColor;

    [SerializeField]
    private float lifetime;

    void OnEnable()
    {
        startTime = Time.time;

        rotationScale.postWrapMode = WrapMode.Loop;
        positionScale.postWrapMode = WrapMode.Loop;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < transform.childCount; i++) {
            // save the new transformation
            fromPlayer = transform.GetChild(i).position - playerTransform.position;

            // get correct distance and vector from player first
            transform.GetChild(i).position = playerTransform.position + fromPlayer.normalized * positionScale.Evaluate((Time.time - startTime) / lifetime);

            // then rotate
            transform.GetChild(i).RotateAround(playerTransform.position, Vector3.up, rotationScale.Evaluate((Time.time - startTime) / lifetime) * rotationSpeed / fromPlayer.magnitude);

            // where to store globals? eg layermask
            PaintingManager.PaintSphere(paintColor, transform.GetChild(i).position, spherePaintRadius);
        }
    }
}
