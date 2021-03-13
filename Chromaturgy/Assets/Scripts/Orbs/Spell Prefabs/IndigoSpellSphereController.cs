using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndigoSpellSphereController : MonoBehaviour
{
    public Orb.GreaterCast greaterCast;
    public Orb.LesserCast lesserCast;
    public int greaterCastAmt;
    public int lesserCastAmt;
    public float spellEffectMod;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Enemy"))
            greaterCast(collision.gameObject, greaterCastAmt, spellEffectMod);
        else if (collision.gameObject.tag.Equals("Player"))
            lesserCast(collision.gameObject, lesserCastAmt, spellEffectMod);
        
        Destroy(gameObject);
    }
}
