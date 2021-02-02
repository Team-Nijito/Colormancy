using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowOrb : Orb
{
    public YellowOrb()
    {
        OrbColor = Color.yellow;
        OrbShape = SpellShape.OrbitingOrbs;
        CooldownMod = 1.3f;
        OrbElement = Element.Light;
        ModAmount = .1f;
    }

    public override void AddHeldEffect(SpellTest test)
    {
        test.HealthRegenMod += ModAmount;
    }

    public override void RevertHeldEffect(SpellTest test)
    {
        test.HealthRegenMod -= ModAmount;
    }

    public override void CastGreaterEffect(GameObject hit, int orbAmount)
    {
        throw new System.NotImplementedException();
    }

    public override void CastLesserEffect(GameObject hit, int orbAmount)
    {
        throw new System.NotImplementedException();
    }

    public override void CastShape(GreaterCast greaterEfectMethod, LesserCast lesserEffectMethod, int greaterEffectAmnt, int lesserEffectAmnt, int shapeAmnt)
    {
        //Cast specific orb shape depending on shapeAmnt
        //For any enemies hit
        //greaterEffectMethod(enemy game object, greaterEffectAmnt);
        //For any allies hit 
        //lesserEffectMethod(ally game object, lesserEffectAmnt);
    }

}
