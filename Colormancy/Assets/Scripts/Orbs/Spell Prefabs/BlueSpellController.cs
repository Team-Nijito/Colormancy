using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueSpellController : MonoBehaviour
{
    private float starttime;
    [SerializeField]
    private float lifetime;
    private const Orb.Element element = Orb.Element.Water;

    [Space]

    [SerializeField]
    private float spherePaintRadius;

    [SerializeField]
    private bool debug;

    void OnEnable()
    {
        starttime = Time.time;

        PaintingManager.PaintSphere(OrbValueManager.getColor(element), transform.position, spherePaintRadius);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - starttime > lifetime && !debug)
            Destroy(gameObject);
    }
}
