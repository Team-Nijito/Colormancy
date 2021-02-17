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
        ShapeManaMod = .9f;
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

    public override void CastShape(GreaterCast greaterEffectMethod, LesserCast lesserEffectMethod, (int, int, int) amounts, Transform t)
    {
        //Cast specific orb shape depending on shapeAmnt
        //For any enemies hit
        //greaterEffectMethod(enemy game object, greaterEffectAmnt);
        //For any allies hit 
        //lesserEffectMethod(ally game object, lesserEffectAmnt);

        GameObject orbs = GameObject.Instantiate(Resources.Load("Orbs/Yellow Orbs", typeof(GameObject))) as GameObject;
        orbs.transform.position = t.position;
        orbs.GetComponent<YellowSpellController>().playerTransform = t;
    }

    public static object Deserialize(byte[] data)
    {
        YellowOrb result = new YellowOrb();
        result.OrbColor = new Color(data[0], data[1], data[2]);
        result.CooldownMod = data[3];
        result.ShapeManaMod = data[4];
        result.ModAmount = data[5];
        return result;
    }

    public static byte[] Serialize(object customType)
    {
        YellowOrb c = (YellowOrb)customType;
        return new byte[] { (byte)c.OrbColor.r, (byte)c.OrbColor.g, (byte)c.OrbColor.b, (byte)c.CooldownMod, (byte)c.ShapeManaMod, (byte)c.ModAmount };
    }
}
