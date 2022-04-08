using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowSpellController : MonoBehaviour
{
    [SerializeField]
    public Transform playerTransform;
    private Vector3 fromPlayer;
    private const Orb.Element element = Orb.Element.Light;

    public bool PVPEnabled = false;
    public PhotonView CasterPView = null;

    [SerializeField]
    private AnimationCurve rotationScale;
    [SerializeField]
    private AnimationCurve positionScale;

    [Space]

    public Orb.GreaterCast greaterCast;
    public Orb.LesserCast lesserCast;
    public float spellEffectMod;

    [Space]

    [SerializeField]
    private float rotationSpeed;
    private float startTime;
    [SerializeField]
    private float lifetime;
   
    [Space]

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
                PaintingManager.PaintSphere(OrbValueManager.getColor(element), transform.GetChild(i).position, OrbValueManager.getPaintRadius(element));
        }

        if (tick == PaintingManager.paintingTickFrequency)
            tick = 0;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Enemy"))
        {
            greaterCast(collider.gameObject, spellEffectMod, null);
        }
        else if (collider.gameObject.CompareTag("Player"))
        {
            if (PVPEnabled && PhotonView.Get(collider.gameObject).ViewID != CasterPView.ViewID)
            {
                greaterCast(collider.gameObject, spellEffectMod, null);
            }
            else
            {
                lesserCast(collider.gameObject, spellEffectMod, null);
            }
        }
    }
}
