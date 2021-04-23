using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
public class BlueOrb : Orb
{
    public BlueOrb()
    {
        m_OrbShape = SpellShape.Ink;
        m_OrbElement = Element.Water;
        m_UIPrefab = (GameObject)Resources.Load("Orbs/BlueOrbUI");
    }

    public override void CastGreaterEffect(GameObject hit, int orbLevel, float spellEffectMod)
    {
        PhotonView photonView = hit.GetPhotonView();
        photonView.RPC("TakeDamage", RpcTarget.All, OrbValueManager.getGreaterEffectDamage(m_OrbElement, m_Level));
    }

    public override void CastLesserEffect(GameObject hit, int orbLevel, float spellEffectMod)
    {
        PhotonView photonView = hit.GetPhotonView();
        photonView.RPC("ManaRegeneration", RpcTarget.All, OrbValueManager.getLesserEffectValue(m_OrbElement, m_Level));
    }

    public override void CastShape(GreaterCast greaterEffectMethod, LesserCast lesserEffectMethod, (int, int, int) amounts, Transform t, Vector3 clickedPosition)
    {
        GameObject g = GameObject.Instantiate(Resources.Load("Orbs/Blue Puddle Spawner"), t.position, t.rotation) as GameObject;
        BlueSpellSpawnerController spellController = g.GetComponent<BlueSpellSpawnerController>();

        spellController.greaterCast = greaterEffectMethod;
        spellController.lesserCast = lesserEffectMethod;
        spellController.greaterCastAmt = amounts.Item1;
        spellController.lesserCastAmt = amounts.Item2;
        spellController.spellEffectMod = OrbValueManager.getSpellEffectMod(m_OrbElement);

        spellController.playerTransform = t;
    }

    public static object Deserialize(byte[] data)
    {
        BlueOrb result = new BlueOrb();
        result.setLevel(data[0]);
        return result;
    }

    public static byte[] Serialize(object customType)
    {
        BlueOrb o = (BlueOrb)customType;
        return new byte[] { (byte)o.getLevel() };
    }

}
