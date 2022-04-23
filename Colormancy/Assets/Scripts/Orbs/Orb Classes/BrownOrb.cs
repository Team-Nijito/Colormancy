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

    public override void AddHeldEffect(GameObject player)
    {
        HealthScript health = player.GetComponent<HealthScript>();
        health.AlterArmorValue(OrbValueManager.getHoldIncreaseValue(m_OrbElement));

        ManaScript mana = player.GetComponent<ManaScript>();
        mana.ChangeManaRegeneration(false, -OrbValueManager.getHoldDecreaseValue(m_OrbElement));
    }

    public override void RevertHeldEffect(GameObject player)
    {
        HealthScript health = player.GetComponent<HealthScript>();
        health.AlterArmorValue(-OrbValueManager.getHoldIncreaseValue(m_OrbElement));

        ManaScript mana = player.GetComponent<ManaScript>();
        mana.ChangeManaRegeneration(true, OrbValueManager.getHoldDecreaseValue(m_OrbElement));
    }

    public override void CastGreaterEffect(GameObject hit, float spellEffectMod, float[] data, Transform casterTransform)
    {
        float dmgMultiplier = 1;
        if (hit.GetComponent<StatusEffectScript>().StatusExists(StatusEffect.StatusType.SpellIncreasedDamage))
            dmgMultiplier += OrbValueManager.getGreaterEffectPercentile(Element.Water) / 100f;

        PhotonView photonView = PhotonView.Get(hit);
        photonView.RPC("TakeDamage", RpcTarget.All, OrbValueManager.getGreaterEffectDamage(m_OrbElement, m_Level) * spellEffectMod * dmgMultiplier);

        StatusEffectScript status = hit.GetComponent<StatusEffectScript>();
        status.RPCApplyStatus(StatusEffect.StatusType.Slowdown, OrbValueManager.getGreaterEffectDuration(m_OrbElement, m_Level), 0, OrbValueManager.getGreaterEffectPercentile(m_OrbElement), "brown_orb");
    }

    public override void CastLesserEffect(GameObject hit, float spellEffectMod, float[] data)
    {
        HealthScript health = hit.GetComponent<HealthScript>();

        PhotonView photonView = PhotonView.Get(hit);
        photonView.RPC("AddShield", RpcTarget.All, OrbValueManager.getLesserEffectValue(m_OrbElement, m_Level) / 100f * health.GetMaxEffectiveHealth());
    }

    public override void CastShape(GreaterCast greaterEffectMethod, LesserCast lesserEffectMethod, Transform t, Vector3 clickedPosition, float spellDamageMultiplier)
    {
        GameObject g = GameObject.Instantiate(Resources.Load("Orbs/Brown Shockwave"), t.position + Vector3.up * 0.03f, t.rotation) as GameObject;
        BrownSpellController spellController = g.GetComponent<BrownSpellController>();

        spellController.greaterCast = greaterEffectMethod;
        spellController.lesserCast = lesserEffectMethod;
        spellController.spellEffectMod = OrbValueManager.getShapeEffectMod(m_OrbElement) * spellDamageMultiplier;
        spellController.PVPEnabled = getPVPStatus();
        spellController.CasterPView = getCasterPView();
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
