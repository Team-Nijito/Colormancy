using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Photon.Pun;
public class QuickSilverOrb : Orb
{
    public QuickSilverOrb()
    {
        OrbColor = new Color(0.58f, 0.62f, 0.66f);
        OrbShape = SpellShape.Bolt;
        CooldownMod = 0.85f;
        ShapeManaMod = 0.85f;
        OrbElement = Element.Wind;
        ModAmount = .1f;
        SpellEffectMod = 1.75f;
        UIPrefab = (GameObject)Resources.Load("Orbs/QuickSilverOrbUI");
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
        Transform wizard = t.GetChild(0);

        Vector3 direction = clickedPosition - t.position;
        wizard.LookAt(wizard.position + new Vector3(direction.x, 0, direction.y).normalized);

        GameObject g = GameObject.Instantiate(Resources.Load("Orbs/QuickSilver Bolt"), clickedPosition + Vector3.up * 0.03f, t.rotation) as GameObject;
        QuickSilverSpellController spellController = g.GetComponent<QuickSilverSpellController>();

        spellController.greaterCast = greaterEffectMethod;
        spellController.lesserCast = lesserEffectMethod;
        spellController.greaterCastAmt = amounts.Item1;
        spellController.lesserCastAmt = amounts.Item2;
        spellController.spellEffectMod = SpellEffectMod;
    }

    public static object Deserialize(byte[] data)
    {
        QuickSilverOrb result = new QuickSilverOrb();
        result.OrbColor = new Color(data[0], data[1], data[2]);
        result.CooldownMod = data[3];
        result.ShapeManaMod = data[4];
        result.ModAmount = data[5];
        return result;
    }

    public static byte[] Serialize(object customType)
    {
        QuickSilverOrb c = (QuickSilverOrb)customType;
        return new byte[] { (byte)c.OrbColor.r, (byte)c.OrbColor.g, (byte)c.OrbColor.b, (byte)c.CooldownMod, (byte)c.ShapeManaMod, (byte)c.ModAmount };
    }
}
