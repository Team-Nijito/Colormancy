using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class BlueOrb : Orb
{
    public BlueOrb()
    {
        OrbColor = Color.blue;
        OrbShape = SpellShape.Ink;
        CooldownMod = 1.8f;
        ShapeManaMod = .8f;
        OrbElement = Element.Water;
        ModAmount = .1f;
        UIPrefab = (GameObject)Resources.Load("Orbs/BlueOrbUI");
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
        PhotonView photonView = hit.GetPhotonView();
        photonView.RPC("TakeDamage", RpcTarget.All, (float)orbAmount);
    }

    public override void CastLesserEffect(GameObject hit, int orbAmount)
    {
        PhotonView photonView = hit.GetPhotonView();
        photonView.RPC("ManaRegeneration", RpcTarget.All, (float)orbAmount);
    }

    public override void CastShape(GreaterCast greaterEffectMethod, LesserCast lesserEffectMethod, (int, int, int) amounts, Transform t, Vector3 clickedPosition)
    {
        //Cast specific orb shape depending on shapeAmnt
        //For any enemies hit
        //greaterEffectMethod(enemy game object, greaterEffectAmnt);
        //For any allies hit 
        //lesserEffectMethod(ally game object, lesserEffectAmnt);
        GameObject g = GameObject.Instantiate(Resources.Load("Orbs/Blue Puddle Spawner"), t.position, t.rotation) as GameObject;
        BlueSpellSpawnerController spellController = g.GetComponent<BlueSpellSpawnerController>();

        spellController.greaterCast = greaterEffectMethod;
        spellController.lesserCast = lesserEffectMethod;
        spellController.greaterCastAmt = amounts.Item1;
        spellController.lesserCastAmt = amounts.Item2;

        spellController.playerTransform = t;
    }

    public static object Deserialize(byte[] data)
    {
        BlueOrb result = new BlueOrb();
        result.OrbColor = new Color(data[0], data[1], data[2]);
        result.CooldownMod = data[3];
        result.ShapeManaMod = data[4];
        result.ModAmount = data[5];
        return result;
    }

    public static byte[] Serialize(object customType)
    {
        BlueOrb c = (BlueOrb)customType;
        return new byte[] { (byte)c.OrbColor.r, (byte)c.OrbColor.g, (byte)c.OrbColor.b, (byte)c.CooldownMod, (byte)c.ShapeManaMod, (byte)c.ModAmount };
    }

}
