using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create new Orb Values")]
public class OrbValueScriptableObject : ScriptableObject
{
    [System.Serializable]
    public struct LeveledValues
    {
        public float m_Level1;
        public float m_Level2;
        public float m_Level3;

        public float getValue(int level) { return level == 1 ? m_Level1 : (level == 2 ? m_Level2 : m_Level3); }
    }

    public Orb.Element element;
    public Color m_OrbColor;
    [Range(0, 10)]
    public float m_PaintRadius;
    public LeveledValues m_GreaterEffectDamage;
    public float m_GreaterEffectPercentile;
    public LeveledValues m_GreaterEffectDuration;
    public LeveledValues m_LesserEffectValue;
    public LeveledValues m_LesserEffectDuration;
    public float m_ShapeEffectMod;
    public float m_CooldownMod;
    public float m_ShapeManaMod;
    [Range(1, 3)]
    public int m_Level;
}
