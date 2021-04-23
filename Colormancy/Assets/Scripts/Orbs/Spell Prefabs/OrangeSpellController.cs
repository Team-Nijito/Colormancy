using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeSpellController : MonoBehaviour
{
    public Orb.GreaterCast greaterCast;
    public Orb.LesserCast lesserCast;
    public int greaterCastLevel;
    public int lesserCastLevel;
    public float spellEffectMod;

    [Space]

    [SerializeField]
    private float speed;
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

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
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

            if (Time.time - startTime > lifetime && !debug)
                Destroy(gameObject);
        }

        PaintingManager.PaintSphere(paintColor, transform.position, spherePaintRadius);
    }

    private void OnCollisionEnter(Collision collision)
    {
        PaintingManager.PaintSphere(paintColor, collision.GetContact(0).point, spherePaintRadius * 2);

        Collider[] sphereCollisions = Physics.OverlapSphere(collision.GetContact(0).point, spherePaintRadius * 2);
        foreach (Collider c in sphereCollisions)
        {
            if (c.gameObject.CompareTag("Enemy"))
                greaterCast(c.gameObject, greaterCastLevel, spellEffectMod);
            else if (c.gameObject.CompareTag("Player"))
                lesserCast(c.gameObject, lesserCastLevel, spellEffectMod);
        }

        Destroy(gameObject);
    }
}
