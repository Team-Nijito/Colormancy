using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeSpellController : MonoBehaviour
{
    [SerializeField]
    private bool debug;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float spherePaintRadius;
    [SerializeField]
    private Color paintColor;

    private float starttime;
    [SerializeField]
    private float lifetime;

    // Start is called before the first frame update
    void Start()
    {
        starttime = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (debug)
        {
            float xpos = 5 * Mathf.Sin(Time.time);
            float ypos = 5 * Mathf.Cos(Time.time);

            transform.position = new Vector3(xpos, 1, ypos);
        }
        else
        {
            transform.position += transform.forward * speed;

            if (Time.time - starttime > lifetime && !debug)
                Destroy(gameObject);
        }

        PaintingManager.PaintSphere(paintColor, transform.position, spherePaintRadius);
    }

    private void OnCollisionEnter(Collision collision)
    {
        PaintingManager.PaintSphere(paintColor, collision.GetContact(0).point, spherePaintRadius * 2);
        Destroy(gameObject);
    }
}
