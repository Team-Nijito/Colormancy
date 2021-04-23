using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Photon.Pun;
public class BrownOrb : Orb
{
    public BrownOrb()
    {
        m_OrbShape = SpellShape.Shockwave;
        m_OrbElement = Element.Earth;
        m_UIPrefab = (GameObject)Resources.Load("Orbs/BrownOrbUI");
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
        //Cast specific orb shape depending on shapeAmnt
        //For any enemies hit
        //greaterEffectMethod(enemy game object, greaterEffectAmnt);
        //For any allies hit 
        //lesserEffectMethod(ally game object, lesserEffectAmnt);
        GameObject g = GameObject.Instantiate(Resources.Load("Orbs/Brown Shockwave"), t.position + Vector3.up * 0.03f, t.rotation) as GameObject;
        BrownSpellController spellController = g.GetComponent<BrownSpellController>();

        spellController.greaterCast = greaterEffectMethod;
        spellController.lesserCast = lesserEffectMethod;
        spellController.greaterCastLevel = levels.Item1;
        spellController.lesserCastLevel = levels.Item2;
        spellController.spellEffectMod = OrbValueManager.getSpellEffectMod(m_OrbElement);
    }

    public static object Deserialize(byte[] data)
    {
        BrownOrb result = new BrownOrb();
        result.setLevel(data[0]);
        return result;
    }

    public static byte[] Serialize(object customType)
    {
        BrownOrb o = (BrownOrb)customType;
        return new byte[] { (byte)o.getLevel() };
    }
}
