using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    #region References for items to use, found on player prefab
    protected PlayerMovement PlayerMovement;
    protected PlayerAttack PlayerAttack;
    protected HealthScript PlayerHealth;
    protected OrbManager PlayerOrbManager;
    protected PhotonView PlayerPhotonView;
    #endregion

    public ItemManager.ItemTypes ItemType { get; protected set; }
    public int MaxItemAmount { get; protected set; }

    /// <summary>
    /// Initialize protected variables for children to use
    /// Should be used before using any of the abstract functions
    /// </summary>
    public virtual void Init(GameObject playerGO)
    {
        //Initialize to instant, but when you make a new item make sure to change this if necessary or else the rest won't work
        ItemType = ItemManager.ItemTypes.Instant;
        //Initialized to very high, for legendary items make sure to change this when overriding Init
        MaxItemAmount = 10;

        PlayerMovement = playerGO.GetComponent<PlayerMovement>();
        PlayerAttack = playerGO.GetComponent<PlayerAttack>();
        PlayerHealth = playerGO.GetComponent<HealthScript>();
        PlayerOrbManager = playerGO.GetComponent<OrbManager>();
        PlayerPhotonView = playerGO.GetPhotonView();
    }

    #region Abstract Functions
    public abstract void AddItemEffects(ItemManager manager);
    public abstract void RemoveItemEffects();

    public virtual float OnHitEffect(float damageValue) { Debug.LogError("This Item does not have an OnHitEffect"); return damageValue; }
    #endregion
}
