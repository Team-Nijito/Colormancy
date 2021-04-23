using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndigoSpellSphereController : MonoBehaviour
{
    public Orb.GreaterCast greaterCast;
    public Orb.LesserCast lesserCast;
    public int greaterCastLevel;
    public int lesserCastLevel;
    public float spellEffectMod;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
            greaterCast(collision.gameObject, greaterCastLevel, spellEffectMod);
        else if (collision.gameObject.CompareTag("Player"))
            lesserCast(collision.gameObject, lesserCastLevel, spellEffectMod);
        
        Destroy(gameObject);
    }
}
