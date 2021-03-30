using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class VioletOrb : Orb
{
    public VioletOrb()
    {
        OrbColor = new Color(0.5f, 0, 0.5f, 1);
        OrbShape = SpellShape.Cloud;
        CooldownMod = 2.2f;
        ShapeManaMod = 1f;
        OrbElement = Element.Poison;
        ModAmount = .1f;
        SpellEffectMod = 0.18f;
        UIPrefab = (GameObject)Resources.Load("Orbs/VioletOrbUI");
    }

    public override void AddHeldEffect(SpellTest test)
    {
        test.HealthRegenMod += ModAmount;
    }

    public override void RevertHeldEffect(SpellTest test)
    {
        test.HealthRegenMod -= ModAmount;
    }

    public override void CastGreaterEffect(GameObject hit, int orbAmount, float spellEffectMod)
    {
        StatusEffectScript status = hit.GetComponent<StatusEffectScript>();
        // currently stacks, should not
        status.RPCApplyOrStackDoT(true, 50 * spellEffectMod, orbAmount * 2 + 3, "Poison");
        status.RPCApplySlowdown("Slow", 10, orbAmount * 2 + 3);
    }

    public override void CastLesserEffect(GameObject hit, int orbAmount, float spellEffectMod)
    {
        throw new System.NotImplementedException();
    }

    public override void CastShape(GreaterCast greaterEffectMethod, LesserCast lesserEffectMethod, (int, int, int) amounts, Transform t, Vector3 clickedPosition)
    {
        //Cast specific orb shape depending on shapeAmnt
        //For any enemies hit
        //greaterEffectMethod(enemy game object, greaterEffectAmnt);
        //For any allies hit 
        //lesserEffectMethod(ally game object, lesserEffectAmnt);
        Vector3 direction = clickedPosition - t.position;

        GameObject g = GameObject.Instantiate(Resources.Load("Orbs/Violet Sphere"), t.position + direction.normalized, t.rotation) as GameObject;
        VioletSpellSphereController spellController = g.GetComponent<VioletSpellSphereController>();

        spellController.greaterCast = greaterEffectMethod;
        spellController.lesserCast = lesserEffectMethod;
        spellController.greaterCastAmt = amounts.Item1;
        spellController.lesserCastAmt = amounts.Item2;
        spellController.spellEffectMod = SpellEffectMod;

        spellController.endPosition = clickedPosition;
    }

    public static object Deserialize(byte[] data) {
        VioletOrb result = new VioletOrb();
        result.OrbColor = new Color(data[0], data[1], data[2]);
        result.CooldownMod = data[3];
        result.ShapeManaMod = data[4];
        result.ModAmount = data[5];
        return result;
    }

    public static byte[] Serialize(object customType) {
        VioletOrb c = (VioletOrb)customType;
        return new byte[] { (byte)c.OrbColor.r, (byte)c.OrbColor.g, (byte)c.OrbColor.b, (byte)c.CooldownMod, (byte)c.ShapeManaMod, (byte)c.ModAmount };
    }

}
