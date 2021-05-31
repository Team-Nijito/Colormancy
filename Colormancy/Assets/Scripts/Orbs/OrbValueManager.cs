using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbValueManager : MonoBehaviour
{
    [System.Serializable]
    public struct LeveledValues
    {
        [SerializeField]
        public float m_Level1;
        [SerializeField]
        public float m_Level2;
        [SerializeField]
        public float m_Level3;

        public float getValue(int level) { return level == 1 ? m_Level1 : (level == 2 ? m_Level2 : m_Level3); }
    }

    [System.Serializable]
    public struct OrbValues
    {
        [SerializeField]
        public Orb.Element element;
        [SerializeField]
        public Color m_OrbColor;
        [SerializeField]
        [Range(0, 10)]
        public float m_PaintRadius;
        [SerializeField]
        public LeveledValues m_GreaterEffectDamage;
        [SerializeField]
        public float m_GreaterEffectPercentile;
        [SerializeField]
        public LeveledValues m_GreaterEffectDuration;
        [SerializeField]
        public LeveledValues m_LesserEffectValue;
        [SerializeField]
        public LeveledValues m_LesserEffectDuration;
        [SerializeField]
        public float m_ShapeEffectMod;
        [SerializeField]
        public float m_CooldownMod;
        [SerializeField]
        public float m_ShapeManaMod;
        [SerializeField]
        [Range(1, 3)]
        public int m_Level;
    }

    public bool debug;
    public List<OrbValues> orbValues;
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
        OrbValues values = s_instance.orbValues.Find(x => x.element == element);
        return s_instance.debug == true ? values.m_GreaterEffectDamage.getValue(values.m_Level) : values.m_GreaterEffectDamage.getValue(level);
    }
    public static float getGreaterEffectPercentile(Orb.Element element)
    {
        return s_instance.orbValues.Find(x => x.element == element).m_GreaterEffectPercentile;
    }
    public static float getGreaterEffectDuration(Orb.Element element, int level)
    {
        OrbValues values = s_instance.orbValues.Find(x => x.element == element);
        return s_instance.debug == true ? values.m_GreaterEffectDuration.getValue(values.m_Level) : values.m_GreaterEffectDuration.getValue(level);
    }
    public static float getLesserEffectValue(Orb.Element element, int level)
    {
        OrbValues values = s_instance.orbValues.Find(x => x.element == element);
        return s_instance.debug == true ? values.m_LesserEffectValue.getValue(values.m_Level) : values.m_LesserEffectValue.getValue(level);
    }
    public static float getLesserEffectDuration(Orb.Element element, int level)
    {
        OrbValues values = s_instance.orbValues.Find(x => x.element == element);
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

    #endregion
}
