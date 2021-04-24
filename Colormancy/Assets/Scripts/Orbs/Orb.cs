using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Orb
{
    [System.Serializable]
    public enum SpellShape
    {
        Jump, Fireball, OrbitingOrbs, Vines, Ink, Cloud, Shockwave, Bolt, ExpandingOrbs
    }

    [System.Serializable]
    public enum Element
    {
        Wrath, Fire, Light, Nature, Water, Poison, Earth, Wind, Darkness
    }

    #region Orb Values
    protected Color m_OrbColor;
    protected SpellShape m_OrbShape;
    protected Element m_OrbElement;
    protected float m_CooldownMod;
    protected float m_ShapeManaMod;
    protected float m_SpellEffectMod;
    protected int m_Level = 1;
    public GameObject m_UIPrefab;
    protected int m_baseDamage;
    #endregion

    // getters and setters for protection
    #region Getters
    public Color getColor() { return m_OrbColor; }
    public Element getElement() { return m_OrbElement; }
    public float getCooldownMod() { return m_CooldownMod; }
    public float getShapeManaMod() { return m_ShapeManaMod; }
    public float getSpellEffectMod() { return m_SpellEffectMod; }
    public int getLevel() { return m_Level; }
    public int getBaseDamage() { return m_baseDamage; }
    #endregion

    #region Setters
    public void setColor( Color c ) { m_OrbColor = c; }
    public void setElement( Element e ) { m_OrbElement = e; }
    public void setCooldownMod( float cd_mod ) { m_CooldownMod = cd_mod; }
    public void setShapeManaMod( float sm_mod ) { m_ShapeManaMod = sm_mod; }
    public void setSpellEffectMod( float se_mod ) { m_SpellEffectMod = se_mod; }
    public void setLevel( int l ) { m_Level = l; }
    public void setBaseDamage( int bd ) { m_baseDamage = bd; }
    #endregion

    //SpellTest will just be the player controller
    public delegate void GreaterCast(GameObject hit, int orbLevel, float spellEffectMod);
    public delegate void LesserCast(GameObject hit, int orbLevel, float spellEffectMod);

    public abstract void CastShape(GreaterCast greaterEffectMethod, LesserCast lesserEffectMethod, (int, int, int) levels, Transform t, Vector3 clickedPosition);
    public abstract void CastGreaterEffect(GameObject hit, int orbAmount, float spellEffectMod);
    //Will have to do something different and send over server for this one since most are for allies
    public abstract void CastLesserEffect(GameObject hit, int orbAmount, float spellEffectMod);
}
