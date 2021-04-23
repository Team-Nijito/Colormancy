using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbValueManager : MonoBehaviour
{
    [System.Serializable]
    public struct LeveledValues
    {
        [SerializeField]
        float m_Level1;
        [SerializeField]
        float m_Level2;
        [SerializeField]
        float m_Level3;
    }

    [System.Serializable]
    public struct OrbValues
    {
        [SerializeField]
        public Orb.Element element;
        [SerializeField]
        public Color m_OrbColor;
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
        public float m_SpellEffectMod;
        [SerializeField]
        public float m_CooldownMod;
        [SerializeField]
        public float m_ShapeManaMod;
    }

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
    public static float getGreaterEffectDamage(Orb.Element element, int level)
    {
        return 20f;
    }
    public static float getGreaterEffectPercentile(Orb.Element element)
    {
        return 20f;
    }
    public static float getGreaterEffectDuration(Orb.Element element, int level)
    {
        return 20f;
    }
    public static float getLesserEffectValue(Orb.Element element, int level)
    {
        return 20f;
    }
    public static float getLesserEffectDuration(Orb.Element element, int level)
    {
        return 20f;
    }
    public static float getSpellEffectMod(Orb.Element element)
    {
        return 1f;
    }
    public static float getCooldownMod(Orb.Element element)
    {
        return 1f;
    }
    public static float getShapeManaMod(Orb.Element element)
    {
        return 1f;
    }

    #endregion
}
