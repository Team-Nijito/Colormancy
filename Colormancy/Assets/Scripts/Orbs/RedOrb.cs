using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Photon.Pun;
public class RedOrb : Orb
{
    public RedOrb()
    {
        m_OrbShape = SpellShape.Jump;
        m_OrbElement = Element.Wrath;
        m_UIPrefab = (GameObject)Resources.Load("Orbs/RedOrbUI");
    }

    public override void CastGreaterEffect(GameObject hit, int orbLevel, float spellEffectMod)
    {
        int trueOrbLevel = orbLevel & 255;
        float launchX = (float)((orbLevel >> 24) & 127) * (((orbLevel >> 31) & 1) == 1 ? -1 : 1);
        float launchY = (float)((orbLevel >> 16) & 127) * (((orbLevel >> 23) & 1) == 1 ? -1 : 1);
        float launchZ = (float)((orbLevel >> 8) & 127) * (((orbLevel >> 15) & 1) == 1 ? -1 : 1);
        Vector3 launchVector = new Vector3(launchX, launchY, launchZ).normalized;

        PhotonView photonView = PhotonView.Get(hit);
        photonView.RPC("TakeDamage", RpcTarget.All, OrbValueManager.getGreaterEffectDamage(m_OrbElement, m_Level));

        float orbDuration = OrbValueManager.getGreaterEffectDuration(m_OrbElement, m_Level);
        StatusEffectScript status = hit.GetComponent<StatusEffectScript>();
        status.RPCApplyStun(orbDuration); // decoupled stun from force, now you need to call stun separately
        status.RPCApplyForce("Knockback", orbDuration, launchVector + Vector3.up, 40f);
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

        GameObject g = GameObject.Instantiate(Resources.Load("Orbs/Red Area"), t.position + Vector3.down, t.rotation) as GameObject;
        RedSpellController spellController = g.GetComponent<RedSpellController>();

        spellController.greaterCast = greaterEffectMethod;
        spellController.lesserCast = lesserEffectMethod;
        spellController.greaterCastLevel = levels.Item1;
        spellController.lesserCastLevel = levels.Item2;
        spellController.spellEffectMod = OrbValueManager.getSpellEffectMod(m_OrbElement);

        spellController.endPosition = clickedPosition + Vector3.up * 1.6f;
        spellController.playerTransform = t;
    }

    public static object Deserialize(byte[] data)
    {
        RedOrb result = new RedOrb();
        result.setLevel(data[0]);
        return result;
    }

    public static byte[] Serialize(object customType)
    {
        RedOrb o = (RedOrb)customType;
        return new byte[] { (byte)o.getLevel() };
    }
}
