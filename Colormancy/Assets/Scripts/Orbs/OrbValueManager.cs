using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbValueManager : MonoBehaviour
{
    public bool debug;
    public List<OrbValueScriptableObject> orbValues;
    private static OrbValueManager s_instance;
    
    // set static object first
    void Awake()
    {
        s_instance = this;
    }

    #region Getters
    public static Color getColor(Orb.Element element)
    {
        return s_instance.orbValues.Find(x => x.element == element).m_OrbColor;
    }
    public static float getPaintRadius(Orb.Element element)
    {
        return s_instance.orbValues.Find(x => x.element == element).m_PaintRadius;
    }
    public static float getGreaterEffectDamage(Orb.Element element, int level)
    {
        OrbValueScriptableObject values = s_instance.orbValues.Find(x => x.element == element);
        return s_instance.debug == true ? values.m_GreaterEffectDamage.getValue(values.m_Level) : values.m_GreaterEffectDamage.getValue(level);
    }
    public static float getGreaterEffectPercentile(Orb.Element element)
    {
        return s_instance.orbValues.Find(x => x.element == element).m_GreaterEffectPercentile;
    }
    public static float getGreaterEffectDuration(Orb.Element element, int level)
    {
        OrbValueScriptableObject values = s_instance.orbValues.Find(x => x.element == element);
        return s_instance.debug == true ? values.m_GreaterEffectDuration.getValue(values.m_Level) : values.m_GreaterEffectDuration.getValue(level);
    }
    public static float getLesserEffectValue(Orb.Element element, int level)
    {
        OrbValueScriptableObject values = s_instance.orbValues.Find(x => x.element == element);
        return s_instance.debug == true ? values.m_LesserEffectValue.getValue(values.m_Level) : values.m_LesserEffectValue.getValue(level);
    }
    public static float getLesserEffectDuration(Orb.Element element, int level)
    {
        OrbValueScriptableObject values = s_instance.orbValues.Find(x => x.element == element);
        return s_instance.debug == true ? values.m_LesserEffectDuration.getValue(values.m_Level) : values.m_LesserEffectDuration.getValue(level);
    }
    public static float getShapeEffectMod(Orb.Element element)
    {
        return s_instance.orbValues.Find(x => x.element == element).m_ShapeEffectMod;
    }
    public static float getCooldownMod(Orb.Element element)
    {
        return s_instance.orbValues.Find(x => x.element == element).m_CooldownMod;
    }
    public static float getShapeManaMod(Orb.Element element)
    {
        return s_instance.orbValues.Find(x => x.element == element).m_ShapeManaMod;
    }
    public static int getLevel(Orb.Element element, int level)
    {
        return s_instance.debug == true ? s_instance.orbValues.Find(x => x.element == element).m_Level : level;
    }
    public static float getHoldIncreaseValue(Orb.Element element)
    {
        return s_instance.orbValues.Find(x => x.element == element).m_HoldIncreaseValue;
    }
    public static float getHoldDecreaseValue(Orb.Element element)
    {
        return s_instance.orbValues.Find(x => x.element == element).m_HoldDecreaseValue;
    }
    #endregion
}
