using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class IndigoOrb : Orb
{
    public IndigoOrb()
    {
        OrbColor = Color.yellow;
        OrbShape = SpellShape.ExpandingOrbs;
        CooldownMod = 1.4f;
        ShapeManaMod = 1.2f;
        OrbElement = Element.Darkness;
        ModAmount = .1f;
        SpellEffectMod = 1f;
        UIPrefab = (GameObject)Resources.Load("Orbs/IndigoOrbUI");
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
        PhotonView photonView = hit.GetPhotonView();
        photonView.RPC("TakeDamage", RpcTarget.All, orbAmount * 20f * spellEffectMod);

        StatusEffectScript status = hit.GetComponent<StatusEffectScript>();
        status.RPCApplyBlind(orbAmount);
        // feared not implemented yet
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

        GameObject g = GameObject.Instantiate(Resources.Load("Orbs/Indigo Orbs", typeof(GameObject))) as GameObject;
        g.transform.position = t.position;

        for (int i = 0; i < 16; i++)
        {
            IndigoSpellSphereController sphereController = g.transform.GetChild(i).GetComponent<IndigoSpellSphereController>();
            sphereController.greaterCast = greaterEffectMethod;
            sphereController.lesserCast = lesserEffectMethod;
            sphereController.greaterCastAmt = amounts.Item1;
            sphereController.lesserCastAmt = amounts.Item2;
            sphereController.spellEffectMod = SpellEffectMod;

            if ((i % 2 == 1 && amounts.Item3 == 2) || (i % 4 != 0 && amounts.Item3 == 1))
                GameObject.Destroy(g.transform.GetChild(i).gameObject);
        }

        IndigoSpellController spellController = g.GetComponent<IndigoSpellController>();
        spellController.playerTransform = t;
    }

    public static object Deserialize(byte[] data)
    {
        IndigoOrb result = new IndigoOrb();
        result.OrbColor = new Color(data[0], data[1], data[2]);
        result.CooldownMod = data[3];
        result.ShapeManaMod = data[4];
        result.ModAmount = data[5];
        return result;
    }

    public static byte[] Serialize(object customType)
    {
        IndigoOrb c = (IndigoOrb)customType;
        return new byte[] { (byte)c.OrbColor.r, (byte)c.OrbColor.g, (byte)c.OrbColor.b, (byte)c.CooldownMod,  (byte)c.ShapeManaMod, (byte)c.ModAmount};
    }
}
