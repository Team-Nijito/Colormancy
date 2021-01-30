using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Orb
{
    public enum SpellShape
    {
        Jump, Fireball, OrbitingOrbs, Vines, Ink, Cloud, Shockwave, Bolt, ExpandingOrbs
    }

    public enum Element
    {
        Wrath, Fire, Light, Nature, Water, Poison, Earth, Wind, Darkness
    }

    public Color OrbColor { get; protected set; }
    public SpellShape OrbShape { get; protected set; }
    public float CooldownMod { get; protected set; }
    public Element OrbElement { get; protected set; }
    protected float ModAmount;

    //SpellTest will just be the player controller
    public delegate void GreaterCast(GameObject hit, int orbAmount);
    public delegate void LesserCast(GameObject hit, int orbAmount);

    public abstract void CastShape(GreaterCast greaterEffectMethod, LesserCast lesserEffectMethod, (int, int, int) amounts, Transform t);
    public abstract void CastGreaterEffect(GameObject hit, int orbAmount);
    //Will have to do something different and send over server for this one since most are for allies
    public abstract void CastLesserEffect(GameObject hit, int orbAmount);
    public abstract void RevertHeldEffect(SpellTest test);
    public abstract void AddHeldEffect(SpellTest test);
}
