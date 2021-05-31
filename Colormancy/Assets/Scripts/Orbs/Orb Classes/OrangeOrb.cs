using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
public class OrangeOrb : Orb
{
    public OrangeOrb()
    {
        m_OrbShape = SpellShape.Fireball;
        m_OrbElement = Element.Fire;
        m_UIPrefab = (GameObject)Resources.Load("Orbs/OrangeOrbUI");
    }

    public override void CastGreaterEffect(GameObject hit, float spellEffectMod, float[] data)
    {
        float dmgMultiplier = 1;
        if (hit.GetComponent<StatusEffectScript>().StatusExists(StatusEffect.StatusType.SpellIncreasedDamage))
            dmgMultiplier += OrbValueManager.getGreaterEffectPercentile(Element.Water) / 100f;

        PhotonView photonView = PhotonView.Get(hit);
        photonView.RPC("TakeDamage", RpcTarget.All, OrbValueManager.getGreaterEffectDamage(m_OrbElement, m_Level) * spellEffectMod * dmgMultiplier);

        StatusEffectScript status = hit.GetComponent<StatusEffectScript>();
        status.RPCApplyStatus(StatusEffect.StatusType.AutoAttackIncreasedDamage, OrbValueManager.getGreaterEffectDuration(m_OrbElement, m_Level), 0, 20);
    }

    public override void CastLesserEffect(GameObject hit, float spellEffectMod, float[] data)
    {
        // PhotonView photonView = PhotonView.Get(hit);

        // right now assume that all cooldowns are reduced by base 30f * modifiers ...
        // photonView.RPC("ReduceAllCooldowns", RpcTarget.All, orbLevel * 30f * spellEffectMod);
    }

    public override void CastShape(GreaterCast greaterEffectMethod, LesserCast lesserEffectMethod, Transform t, Vector3 clickedPosition)
    {
        // get the wizard rotation
        Transform wizard = t.GetChild(0);

        Vector3 direction = new Vector3(clickedPosition.x - t.position.x, 0, clickedPosition.z - t.position.z).normalized;
        wizard.LookAt(wizard.position + direction);

        GameObject g = GameObject.Instantiate(Resources.Load("Orbs/Orange Fireball"), t.position + direction, wizard.rotation) as GameObject;
        OrangeSpellController spellController = g.GetComponent<OrangeSpellController>();

        spellController.greaterCast = greaterEffectMethod;
        spellController.lesserCast = lesserEffectMethod;
        spellController.spellEffectMod = OrbValueManager.getShapeEffectMod(m_OrbElement);
    }

    public static object Deserialize(byte[] data)
    {
        OrangeOrb result = new OrangeOrb();
        result.setLevel(data[0]);
        return result;
    }

    public static byte[] Serialize(object customType)
    {
        OrangeOrb o = (OrangeOrb)customType;
        return new byte[] { (byte)o.getLevel() };
    }
}
