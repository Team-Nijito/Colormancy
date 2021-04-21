using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class VioletOrb : Orb
{
    public VioletOrb()
    {
        m_OrbShape = SpellShape.Cloud;
        m_OrbElement = Element.Poison;
        m_UIPrefab = (GameObject)Resources.Load("Orbs/VioletOrbUI");
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
        spellController.spellEffectMod = m_SpellEffectMod;

        spellController.endPosition = clickedPosition;
    }

    public static object Deserialize(byte[] data) {
        VioletOrb result = new VioletOrb();
        result.setColor(new Color(data[0], data[1], data[2]));
        result.setCooldownMod(data[3]);
        result.setShapeManaMod(data[4]);
        result.setSpellEffectMod(data[5]);
        return result;
    }

    public static byte[] Serialize(object customType) {
        VioletOrb o = (VioletOrb)customType;
        return new byte[] { (byte)o.getColor().r, (byte)o.getColor().g, (byte)o.getColor().b, (byte)o.getCooldownMod(), (byte)o.getShapeManaMod(), (byte)o.getSpellEffectMod() };
    }

}
