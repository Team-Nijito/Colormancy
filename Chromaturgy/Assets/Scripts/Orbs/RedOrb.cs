using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedOrb : Orb
{
    public RedOrb()
    {
        OrbColor = Color.red;
        OrbShape = SpellShape.Jump;
        CooldownMod = .7f;
        OrbElement = Element.Wrath;
        ModAmount = .1f;
    }

    public override void AddHeldEffect(SpellTest test)
    {
        test.AttackSpeedMod += ModAmount;
    }

    public override void RevertHeldEffect(SpellTest test)
    {
        test.AttackSpeedMod -= ModAmount;
    }

    public override void CastGreaterEffect(GameObject hit, int orbAmount)
    {
        throw new System.NotImplementedException();
    }

    public override void CastLesserEffect(GameObject hit, int orbAmount)
    {
        throw new System.NotImplementedException();
    }

    public override void CastShape(GreaterCast greaterEffectMethod, LesserCast lesserEffectMethod, int greaterEffectAmnt, int lesserEffectAmnt, int shapeAmnt)
    {
        //Cast specific orb shape depending on shapeAmnt
        //For any enemies hit
        //greaterEffectMethod(enemy game object, greaterEffectAmnt);
        //For any allies hit 
        //lesserEffectMethod(ally game object, lesserEffectAmnt);

        GameObject g = Object.Instantiate(Resources.Load("Orb/RedOrb", typeof(GameObject))) as GameObject;
    }
}
