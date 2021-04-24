using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class IndigoOrb : Orb
{
    public IndigoOrb()
    {
        /*
        OrbColor = Color.yellow;
        CooldownMod = 1.4f;
        ShapeManaMod = 1.2f;
        ModAmount = .1f;
        SpellEffectMod = 1f;
        */
        m_OrbShape = SpellShape.ExpandingOrbs;
        m_OrbElement = Element.Darkness;
        m_UIPrefab = (GameObject)Resources.Load("Orbs/IndigoOrbUI");
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
            sphereController.spellEffectMod = m_SpellEffectMod;

            if ((i % 2 == 1 && amounts.Item3 == 2) || (i % 4 != 0 && amounts.Item3 == 1))
                GameObject.Destroy(g.transform.GetChild(i).gameObject);
        }

        IndigoSpellController spellController = g.GetComponent<IndigoSpellController>();
        spellController.playerTransform = t;
    }

    public static object Deserialize(byte[] data)
    {
        IndigoOrb result = new IndigoOrb();
        result.setColor(new Color(data[0], data[1], data[2]));
        result.setCooldownMod(data[3]);
        result.setShapeManaMod(data[4]);
        result.setSpellEffectMod(data[5]);
        return result;
    }

    public static byte[] Serialize(object customType)
    {
        IndigoOrb o = (IndigoOrb)customType;
        return new byte[] { (byte)o.getColor().r, (byte)o.getColor().g, (byte)o.getColor().b, (byte)o.getCooldownMod(), (byte)o.getShapeManaMod(), (byte)o.getSpellEffectMod() };
    }
}
