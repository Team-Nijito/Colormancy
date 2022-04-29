using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviourPun
{

    //Dictionary Keys are the items and the Values is the amount of that item the player has
    Dictionary<Item, int> instantItems = new Dictionary<Item, int>();
    Dictionary<Item, int> onHitItems = new Dictionary<Item, int>();
    Dictionary<Item, int> damageMultiplierItems = new Dictionary<Item, int>();
    Dictionary<Item, int> spellCastItems = new Dictionary<Item, int>();
    Dictionary<Item, int> onAutoAttackItems = new Dictionary<Item, int>();
    Dictionary<Item, int> onKeyPressed = new Dictionary<Item, int>();

    [SerializeField] GameObject aegisGraphic;
    [SerializeField] GameObject itemParent;

    readonly bool DebugMode = true;

    SpawnpointBehaviour[] spawnpoints;

    //For debug purposes
    Item lastAddedItem;

    #region MonoBehaviour Methods
    // Start is called before the first frame update
    void Start()
    {
        photonView.RPC("AddItem", RpcTarget.All, "L_SUPrincipal");

        spawnpoints = FindObjectsOfType<SpawnpointBehaviour>();
        foreach(SpawnpointBehaviour spawnpoint in spawnpoints)
        {
            //spawnpoint.spawnedEnemy.AddListener
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    if (lastAddedItem != null)
        //    {
        //        RemoveItem(lastAddedItem);
        //        lastAddedItem = null;
        //    }
        //}

        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    if (photonView.IsMine)
        //    {
        //        photonView.RPC("OnHit", RpcTarget.All, 0f);
        //    }
        //}

        foreach(Item item in onKeyPressed.Keys)
        {
            if (Input.GetKeyDown(item.KeyToPress))
            {
                item.OnKeyPressed();
            }
        }
    }
    #endregion

    #region Public Methods
    public void RPCAddItem(string itemName)
    {
        photonView.RPC("AddItem", RpcTarget.All, itemName);
    }

    public float DoDamageMultipliers(float spellDmgMultiplier)
    {
        foreach(Item item in damageMultiplierItems.Keys)
        {
            spellDmgMultiplier = item.DoDamageMultiplier(spellDmgMultiplier);
        }

        return spellDmgMultiplier;
    }

    public void OnSpellCast()
    {
        if (DebugMode)
        {
            Debug.Log("Spell Cast Effects Triggered");
        }
        foreach(Item item in spellCastItems.Keys)
        {
            item.OnSpellCast();
        }
    }

    public void OnAutoAttack(GameObject shotAuto)
    {
        if (DebugMode)
            Debug.Log("Auto attack effects triggered");

        foreach(Item item in onAutoAttackItems.Keys)
        {
            item.OnAutoAttack(shotAuto);
        }
    }
    #endregion


    #region RPC Methods
    [PunRPC]
    public void AddItem(string itemName)
    {
        if (!photonView.IsMine)
            return;
        if (DebugMode)
            Debug.Log($"Adding item for {itemName}");
        Item item = (Item)itemParent.AddComponent(System.Type.GetType(itemName));
        item.Init(gameObject);
        item.AddItemEffects(this);
        AddItemToDictionaries(item);
        lastAddedItem = item;
    }

    [PunRPC]
    public void RemoveItem(Item item)
    {
        RemoveItemFromDictionaries(item);
    }

    /// <summary>
    /// Handles item effects that happen when player takes damage.
    /// </summary>
    /// <param name="damageValue">Original damage value</param>
    /// <returns>Modified damage value after item effects</returns>
    [PunRPC]
    public float OnTakeDamage(float damageValue)
    {
        if (!photonView.IsMine)
            return damageValue;
        float modifiedDamageValue = damageValue;
        if (DebugMode)
            Debug.Log("On Hit Effects Triggered");
        foreach(Item item in onHitItems.Keys)
        {
            if (DebugMode)
                Debug.Log($"ItemManager::OnTakeDamage - Triggering {item}");
            modifiedDamageValue = item.OnTakeDamage(modifiedDamageValue);
        }

        return modifiedDamageValue;
    }

    [PunRPC]
    public void EnableAegis()
    {
        aegisGraphic.SetActive(true);
    }

    [PunRPC]
    public void DisableAegis()
    {
        aegisGraphic.SetActive(false);
    }
    #endregion

    #region Private Methods
    void AddItemToDictionaries(Item item)
    {
        foreach(Item.ItemTypes itemType in item.Types)
        {
            switch (itemType)
            {
                case Item.ItemTypes.Instant:
                    if (DebugMode)
                        Debug.Log("ItemManager::AddItemToDictionaries - Adding to instant items");
                    CheckAndAdd(item, instantItems);
                    break;
                case Item.ItemTypes.DamageTaken:
                    if (DebugMode)
                        Debug.Log("ItemManager::AddItemToDictionaries - Adding to on hit items");
                    CheckAndAdd(item, onHitItems);
                    break;
                case Item.ItemTypes.DamageMultiplier:
                    if (DebugMode)
                        Debug.Log("ItemManager::AddItemToDictionaries - Adding to damage multiplier items");
                    CheckAndAdd(item, damageMultiplierItems);
                    break;
                case Item.ItemTypes.SpellCast:
                    if (DebugMode)
                        Debug.Log("ItemManager::AddItemToDictionaries - Adding to spell cast items");
                    CheckAndAdd(item, spellCastItems);
                    break;
                case Item.ItemTypes.OnAutoAttack:
                    if (DebugMode)
                        Debug.Log("ItemManager::AddItemToDictionaries - Adding to on auto attack items");
                    CheckAndAdd(item, onAutoAttackItems);
                    break;
                case Item.ItemTypes.OnKeyPressed:
                    if (DebugMode)
                        Debug.Log("ItemManager::AddItemToDictionaries - Adding to on key pressed items");
                    CheckAndAdd(item, onKeyPressed);
                    break;
                default:
                    Debug.LogError($"ItemManager::AddItemToDictionaries - Not yet implemented item type {item.Types}");
                    break;
            }
        }
    }

    void RemoveItemFromDictionaries(Item item)
    {
        foreach(Item.ItemTypes itemType in item.Types)
        {
            switch (itemType)
            {
                case Item.ItemTypes.Instant:
                    if (DebugMode)
                        Debug.Log("ItemManager::RemoveItemFromDictionaries - Removing from instant items");
                    HandleRemovingItem(item, instantItems);
                    break;
                case Item.ItemTypes.DamageTaken:
                    if (DebugMode)
                        Debug.Log("ItemManager::RemoveItemFromDictionaries - Removing from on hit items");
                    HandleRemovingItem(item, onHitItems);
                    break;
                case Item.ItemTypes.DamageMultiplier:
                    if (DebugMode)
                        Debug.Log("ItemManager::RemoveItemFromDictionaries - Removing from damage multiplier items");
                    HandleRemovingItem(item, damageMultiplierItems);
                    break;
                case Item.ItemTypes.SpellCast:
                    if (DebugMode)
                        Debug.Log("ItemManager::RemoveItemFromDictionaries - Removing from spell cast items");
                    HandleRemovingItem(item, spellCastItems);
                    break;
                case Item.ItemTypes.OnAutoAttack:
                    if (DebugMode)
                        Debug.Log("ItemManager::RemoveItemFromDictionaries - Removing from on auto attack items");
                    HandleRemovingItem(item, onAutoAttackItems);
                    break;
                case Item.ItemTypes.OnKeyPressed:
                    if (DebugMode)
                        Debug.Log("ItemManager::RemoveItemFromDictionaries - Removing from on key pressed items");
                    HandleRemovingItem(item, onKeyPressed);
                    break;
                default:
                    Debug.LogError($"ItemManager::RemoveItemFromDictionaries - Not yet implemented item type {item.Types}");
                    break;
            }
        }
    }

    /// <summary>
    /// Does dirty work for <seealso cref="AddItemToDictionaries(Item)">AddItemToDictionary</seealso>.
    /// Checking if key already exists and checking if the item can be allowed in limits
    /// </summary>
    /// <param name="item">Item to add</param>
    /// <param name="itemDictionary">Item dictionary to add to</param>
    void CheckAndAdd(Item item, Dictionary<Item, int> itemDictionary)
    {
        if (itemDictionary.ContainsKey(item))
        {
            if (CheckItemLimits(item, itemDictionary))
            {
                //Not yet implemented
                //itemDictionary[item].DoAdditiveEffect();
                itemDictionary[item] += 1;
            }
        }
        else
        {
            itemDictionary.Add(item, 1);
        }
    }

    void HandleRemovingItem(Item item, Dictionary<Item, int> itemDictionary)
    {
        if (itemDictionary.ContainsKey(item))
        {
            if (itemDictionary[item] > 1)
            {
                //Not yet implemented
                //item.RemoveAdditiveEffect();
                itemDictionary[item] -= 1;
            }else
            {
                item.RemoveItemEffects();
                itemDictionary.Remove(item);
            }
        }else
        {
            Debug.LogError($"ItemManager::HandleRemovingItem - {item} not in dictionary");
        }
    }

    /// <summary>
    /// Give an item and a item dictionary checks if the item can be added and not go over item limit
    /// </summary>
    /// <param name="itemToCheck">Item to check</param>
    /// <param name="itemDictionary">Corresponding item dictionary</param>
    /// <returns>True if you can add the item, False otherwise</returns>
    bool CheckItemLimits(Item itemToCheck, Dictionary<Item, int> itemDictionary)
    {
        //Error checking
        if (itemDictionary.ContainsKey(itemToCheck))
        {
            int currentItemAmount = itemDictionary[itemToCheck];

            if (currentItemAmount < itemToCheck.MaxItemAmount)
            {
                return true;
            }else
            {
                return false;
            }
        }else
        {
            Debug.LogError("ItemManager::CheckItemLimits - Passed item that does not exist");
            return false;
        }
    }
    #endregion
}
