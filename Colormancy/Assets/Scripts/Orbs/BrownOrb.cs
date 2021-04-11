using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Photon.Pun;
public class BrownOrb : Orb
{
    public BrownOrb()
    {
        OrbColor = new Color(0.54f, 0.27f, 0.07f);
        OrbShape = SpellShape.Shockwave;
        CooldownMod = 1.7f;
        ShapeManaMod = 1.2f;
        OrbElement = Element.Earth;
        ModAmount = .1f;
        SpellEffectMod = 1.75f;
        UIPrefab = (GameObject)Resources.Load("Orbs/BrownOrbUI");
    }

    public override void AddHeldEffect(SpellTest test)
    {
        test.AttackSpeedMod += ModAmount;
    }

    public override void RevertHeldEffect(SpellTest test)
    {
        test.AttackSpeedMod -= ModAmount;
    }

    public override void CastGreaterEffect(GameObject hit, int orbAmount, float spellEffectMod)
    {
        PhotonView photonView = PhotonView.Get(hit);
        photonView.RPC("TakeDamage", RpcTarget.All, orbAmount * 20f * spellEffectMod);
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
        GameObject g = GameObject.Instantiate(Resources.Load("Orbs/Brown Shockwave"), t.position + Vector3.up * 0.03f, t.rotation) as GameObject;
        BrownSpellController spellController = g.GetComponent<BrownSpellController>();

        spellController.greaterCast = greaterEffectMethod;
        spellController.lesserCast = lesserEffectMethod;
        spellController.greaterCastAmt = amounts.Item1;
        spellController.lesserCastAmt = amounts.Item2;
        spellController.spellEffectMod = SpellEffectMod;
    }

    public static object Deserialize(byte[] data)
    {
        BrownOrb result = new BrownOrb();
        result.OrbColor = new Color(data[0], data[1], data[2]);
        result.CooldownMod = data[3];
        result.ShapeManaMod = data[4];
        result.ModAmount = data[5];
        return result;
    }

    public static byte[] Serialize(object customType)
    {
        BrownOrb c = (BrownOrb)customType;
        return new byte[] { (byte)c.OrbColor.r, (byte)c.OrbColor.g, (byte)c.OrbColor.b, (byte)c.CooldownMod, (byte)c.ShapeManaMod, (byte)c.ModAmount };
    }
}
