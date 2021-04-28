using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Photon.Pun;
public class GreenOrb : Orb
{
    public GreenOrb()
    {
        m_OrbShape = SpellShape.Vines;
        m_OrbElement = Element.Nature;
        m_UIPrefab = (GameObject)Resources.Load("Orbs/GreenOrbUI");
    }

    public override void CastGreaterEffect(GameObject hit, float spellEffectMod, float[] data)
    {
        PhotonView photonView = PhotonView.Get(hit);
        photonView.RPC("TakeDamage", RpcTarget.All, OrbValueManager.getGreaterEffectDamage(m_OrbElement, m_Level) * spellEffectMod);
    }

    public override void CastLesserEffect(GameObject hit, float spellEffectMod, float[] data)
    {
        throw new System.NotImplementedException();
    }

    public override void CastShape(GreaterCast greaterEffectMethod, LesserCast lesserEffectMethod, Transform t, Vector3 clickedPosition)
    {
        Transform wizard = t.GetChild(0);

        Vector3 direction = new Vector3(clickedPosition.x - t.position.x, 0, clickedPosition.z - t.position.z).normalized;
        wizard.LookAt(wizard.position + direction);

        GameObject g = GameObject.Instantiate(Resources.Load("Orbs/Green Vine Spawner"), t.position, wizard.rotation) as GameObject;
        GreenSpellSpawnerController spellController = g.GetComponent<GreenSpellSpawnerController>();

        spellController.greaterCast = greaterEffectMethod;
        spellController.lesserCast = lesserEffectMethod;
        spellController.spellEffectMod = OrbValueManager.getShapeEffectMod(m_OrbElement);
    }

    public static object Deserialize(byte[] data)
    {
        GreenOrb result = new GreenOrb();
        result.setLevel(data[0]);
        return result;
    }

    public static byte[] Serialize(object customType)
    {
        GreenOrb o = (GreenOrb)customType;
        return new byte[] { (byte)o.getLevel() };
    }
}
