using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public enum ItemTypes { Instant, DamageTaken, DamageMultiplier, SpellCast, OnAutoAttack, OnKeyPressed, DamageDealt};

    #region References for items to use, found on player prefab
    protected PlayerMovement PlayerMovement;
    protected PlayerAttack PlayerAttack;
    protected PlayerProjectileSpawner PlayerProjectileSpawner;
    protected HealthScript PlayerHealth;
    protected OrbManager PlayerOrbManager;
    protected PhotonView PlayerPhotonView;
    #endregion

    public List<ItemTypes> Types { get; protected set; }
    public int MaxItemAmount { get; protected set; }

    public KeyCode KeyToPress { get; protected set; }

    /// <summary>
    /// Initialize protected variables for children to use
    /// Should be used before using any of the abstract functions
    /// </summary>
    public virtual void Init(GameObject playerGO)
    {
        //Initialize to instant, but when you make a new item make sure to change this if necessary or else the rest won't work
        Types = new List<ItemTypes>();
        Types.Add(ItemTypes.Instant);
        //Initialized to very high, for legendary items make sure to change this when overriding Init
        MaxItemAmount = 10;

        PlayerMovement = playerGO.GetComponent<PlayerMovement>();
        PlayerAttack = playerGO.GetComponent<PlayerAttack>();
        PlayerProjectileSpawner = playerGO.GetComponent<PlayerProjectileSpawner>();
        PlayerHealth = playerGO.GetComponent<HealthScript>();
        PlayerOrbManager = playerGO.GetComponent<OrbManager>();
        PlayerPhotonView = playerGO.GetPhotonView();
    }

    #region Abstract Functions
    public abstract void AddItemEffects(ItemManager manager);
    public abstract void RemoveItemEffects();

    public virtual float OnTakeDamage(float damageValue) { Debug.LogError("This Item does not have an OnHitEffect"); return damageValue; }
    public virtual float DoDamageMultiplier(float baseDamageMultiplier) { Debug.LogError("This Item does not have a DamageMultiplier effect"); return baseDamageMultiplier; }
    public virtual void OnSpellCast() { Debug.LogError("This Item does not have a SpellCast effect"); }
    public virtual void OnAutoAttack(GameObject shotAuto) { Debug.LogError("This Item does not have an on auto attack effect"); }
    public virtual void OnKeyPressed() { Debug.LogError("This Item does not have an on key pressed effect"); }
    public virtual void OnDamageDealt(GameObject hit, Transform sourcePosition) { Debug.LogError("This Item does not have an on damage dealt effect"); }
    #endregion
}
