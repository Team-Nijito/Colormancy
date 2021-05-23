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

    public override void CastGreaterEffect(GameObject hit, float spellEffectMod, float[] data)
    {
        float dmgMultiplier = 1;
        if (hit.GetComponent<StatusEffectScript>().StatusExists(StatusEffect.StatusType.SpellIncreasedDamage))
            dmgMultiplier += OrbValueManager.getGreaterEffectPercentile(Element.Water) / 100f;

        float vector_x = 0;
        float vector_y = 0;
        float vector_z = 0;
        if (data != null)
        {
            vector_x = data[0];
            vector_y = data[1];
            vector_z = data[2];
        }
        Vector3 launchVector = new Vector3(vector_x, vector_y, vector_z);

        PhotonView photonView = PhotonView.Get(hit);
        photonView.RPC("TakeDamage", RpcTarget.All, OrbValueManager.getGreaterEffectDamage(m_OrbElement, m_Level) * spellEffectMod * dmgMultiplier);

        float orbDuration = OrbValueManager.getGreaterEffectDuration(m_OrbElement, m_Level);
        StatusEffectScript status = hit.GetComponent<StatusEffectScript>();
        status.RPCApplyStatus(StatusEffect.StatusType.Stun, OrbValueManager.getGreaterEffectDuration(m_OrbElement, m_Level));
        status.RPCApplyForce(orbDuration, "red_orb", launchVector + Vector3.up, 40f);
    }

    public override void CastLesserEffect(GameObject hit, float spellEffectMod, float[] data)
    {
        
    }

    public override void CastShape(GreaterCast greaterEffectMethod, LesserCast lesserEffectMethod, Transform t, Vector3 clickedPosition)
    {
        Transform wizard = t.GetChild(0);

        Vector3 direction = clickedPosition - t.position;
        wizard.LookAt(wizard.position + new Vector3(direction.x, 0, direction.y).normalized);

        GameObject g = GameObject.Instantiate(Resources.Load("Orbs/Red Area"), t.position + Vector3.down, t.rotation) as GameObject;
        RedSpellController spellController = g.GetComponent<RedSpellController>();

        spellController.greaterCast = greaterEffectMethod;
        spellController.lesserCast = lesserEffectMethod;
        spellController.spellEffectMod = OrbValueManager.getShapeEffectMod(m_OrbElement);

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
