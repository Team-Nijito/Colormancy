using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Photon.Pun;
public class QuickSilverOrb : Orb
{
    public QuickSilverOrb()
    {
        m_OrbShape = SpellShape.Bolt;
        m_OrbElement = Element.Wind;
        m_UIPrefab = (GameObject)Resources.Load("Orbs/QuickSilverOrbUI");
    }

    public override void CastGreaterEffect(GameObject hit, int orbLevel, float spellEffectMod)
    {
        PhotonView photonView = PhotonView.Get(hit);
        photonView.RPC("TakeDamage", RpcTarget.All, OrbValueManager.getGreaterEffectDamage(m_OrbElement, m_Level));
    }

    public override void CastLesserEffect(GameObject hit, int orbLevel, float spellEffectMod)
    {
        throw new System.NotImplementedException();
    }

    public override void CastShape(GreaterCast greaterEffectMethod, LesserCast lesserEffectMethod, (int, int, int) levels, Transform t, Vector3 clickedPosition)
    {
        Transform wizard = t.GetChild(0);

        Vector3 direction = clickedPosition - t.position;
        wizard.LookAt(wizard.position + new Vector3(direction.x, 0, direction.y).normalized);

        GameObject g = GameObject.Instantiate(Resources.Load("Orbs/QuickSilver Bolt"), clickedPosition + Vector3.up * 0.03f, t.rotation) as GameObject;
        QuickSilverSpellController spellController = g.GetComponent<QuickSilverSpellController>();

        spellController.greaterCast = greaterEffectMethod;
        spellController.lesserCast = lesserEffectMethod;
        spellController.greaterCastLevel = levels.Item1;
        spellController.lesserCastLevel = levels.Item2;
        spellController.spellEffectMod = OrbValueManager.getSpellEffectMod(m_OrbElement);
    }

    public static object Deserialize(byte[] data)
    {
        QuickSilverOrb result = new QuickSilverOrb();
        result.setLevel(data[0]);
        return result;
    }

    public static byte[] Serialize(object customType)
    {
        QuickSilverOrb o = (QuickSilverOrb)customType;
        return new byte[] { (byte)o.getLevel() };
    }
}
