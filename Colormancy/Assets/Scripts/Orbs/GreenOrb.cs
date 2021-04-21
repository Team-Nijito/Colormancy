using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Photon.Pun;
public class GreenOrb : Orb
{
    public GreenOrb()
    {
        /*
        OrbColor = Color.green;
        CooldownMod = 2.2f;
        ShapeManaMod = 1.1f;
        ModAmount = .1f;
        SpellEffectMod = 0.18f;
        */
        m_OrbShape = SpellShape.Vines;
        m_OrbElement = Element.Nature;
        m_UIPrefab = (GameObject)Resources.Load("Orbs/GreenOrbUI");
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

        Vector3 direction = new Vector3(clickedPosition.x - t.position.x, 0, clickedPosition.z - t.position.z).normalized;
        wizard.LookAt(wizard.position + direction);

        GameObject g = GameObject.Instantiate(Resources.Load("Orbs/Green Vine Spawner"), t.position, wizard.rotation) as GameObject;
        GreenSpellSpawnerController spellController = g.GetComponent<GreenSpellSpawnerController>();

        spellController.greaterCast = greaterEffectMethod;
        spellController.lesserCast = lesserEffectMethod;
        spellController.greaterCastAmt = amounts.Item1;
        spellController.lesserCastAmt = amounts.Item2;
        spellController.spellEffectMod = m_SpellEffectMod;
    }

    public static object Deserialize(byte[] data)
    {
        GreenOrb result = new GreenOrb();
        result.setColor(new Color(data[0], data[1], data[2]));
        result.setCooldownMod(data[3]);
        result.setShapeManaMod(data[4]);
        result.setSpellEffectMod(data[5]);
        return result;
    }

    public static byte[] Serialize(object customType)
    {
        GreenOrb o = (GreenOrb)customType;
        return new byte[] { (byte)o.getColor().r, (byte)o.getColor().g, (byte)o.getColor().b, (byte)o.getCooldownMod(), (byte)o.getShapeManaMod(), (byte)o.getSpellEffectMod() };
    }
}
