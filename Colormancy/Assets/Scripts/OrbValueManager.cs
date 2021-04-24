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
        string m_name;
        [SerializeField]
        Color m_OrbColor;
        [SerializeField]
        LeveledValues m_GreaterEffectDamage;
        [SerializeField]
        float m_GreaterEffectPercentile;
        [SerializeField]
        LeveledValues m_GreaterEffectDuration;
        [SerializeField]
        LeveledValues m_LesserEffectValue;
        [SerializeField]
        LeveledValues m_LesserEffectDuration;
        [SerializeField]
        float m_SpellEffectMod;
        [SerializeField]
        float m_CooldownMod;
        [SerializeField]
        float m_ShapeManaMod;
    }

    public List<OrbValues> orbValues;
    public static OrbValueManager s_instance;

    void Awake()
    {
        s_instance = this;
    }
}
