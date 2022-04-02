using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    #region Loot Table Variables
    protected float itemWeight = 1f; //Used in loot tables initialize everything to base value of 1
    #endregion

    #region References for items to use, found on player prefab
    protected PlayerMovement playerMovement;
    #endregion

    /// <summary>
    /// Initialize protected variables for children to use
    /// Should be used before using any of the abstract functions
    /// </summary>
    public void Init()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    public float GetWeight()
    {
        return itemWeight;
    }
    #region Abstract Functions
    public abstract void AddItemEffects();
    public abstract void RemoveItemEffects();
    #endregion
}
