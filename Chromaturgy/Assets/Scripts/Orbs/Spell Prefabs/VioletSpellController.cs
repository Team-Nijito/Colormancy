using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VioletSpellController : MonoBehaviour
{
    private float starttime;
    [SerializeField]
    private float lifetime;

    [SerializeField]
    private float spherePaintRadius;
    [SerializeField]
    private Color paintColor;

    [SerializeField]
    private bool debug;

    void OnEnable()
    {
        starttime = Time.time;

        PaintingManager.PaintSphere(paintColor, transform.position + Vector3.down, spherePaintRadius);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - starttime > lifetime && !debug)
            Destroy(gameObject);
    }
}
