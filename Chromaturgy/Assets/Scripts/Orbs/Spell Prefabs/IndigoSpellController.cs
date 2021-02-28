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

    private float startTime;
    [SerializeField]
    private float lifetime;

    [SerializeField]
    private float spherePaintRadius;
    [SerializeField]
    private Color paintColor;

    [SerializeField]
    private bool debug;

    private int tick;

    public Orb.GreaterCast greaterCast;
    public Orb.LesserCast lesserCast;
    public int greaterCastAmt;
    public int lesserCastAmt;

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

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag.Equals("Enemy"))
        {
            greaterCast(collision.gameObject, greaterCastAmt);
        }

    }
}
