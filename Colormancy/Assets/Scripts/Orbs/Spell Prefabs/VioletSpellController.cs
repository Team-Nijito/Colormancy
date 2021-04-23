using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VioletSpellController : MonoBehaviour
{
    public Orb.GreaterCast greaterCast;
    public Orb.LesserCast lesserCast;
    public int greaterCastLevel;
    public int lesserCastLevel;
    public float spellEffectMod;
    private const Orb.Element element = Orb.Element.Poison;

    [Space]

    private float starttime;
    [SerializeField]
    private float lifetime;

    [Space]

    [SerializeField]
    private float spherePaintRadius;
    [SerializeField]
    private Color paintColor;

    [SerializeField]
    private bool debug;

    void OnEnable()
    {
        starttime = Time.time;

        PaintingManager.PaintSphere(OrbValueManager.getColor(element), transform.position + Vector3.down, spherePaintRadius);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - starttime > lifetime && !debug)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            greaterCast(collision.gameObject, greaterCastLevel, spellEffectMod);
        }
        else if (collision.gameObject.CompareTag("Player"))
            lesserCast(collision.gameObject, lesserCastLevel, spellEffectMod);
    }
}
