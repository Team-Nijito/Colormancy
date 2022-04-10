using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviourPun
{
    public enum ItemTypes { Instant, OnHit };

    //Dictionary Keys are the items and the Values is the amount of that item the player has
    Dictionary<Item, int> instantItems = new Dictionary<Item, int>();
    Dictionary<Item, int> onHitItems = new Dictionary<Item, int>();

    [SerializeField] GameObject itemParent;

    readonly bool DebugMode = true;

    //For debug purposes
    Item lastAddedItem;

    // Start is called before the first frame update
    void Start()
    {
        AddItem("C_SplatJacket");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (lastAddedItem != null)
            {
                RemoveItem(lastAddedItem);
                lastAddedItem = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            photonView.RPC("OnHit", RpcTarget.All, 0f);
        }
    }

    public void AddItem(string itemName)
    {
        if (DebugMode)
            Debug.Log($"Adding item for {itemName}");
        Item item = (Item)itemParent.AddComponent(System.Type.GetType(itemName));
        item.Init(gameObject);
        item.AddItemEffects(this);
        AddItemToDictionaries(item);
        lastAddedItem = item;
    }

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
    public float OnHit(float damageValue)
    {
        float modifiedDamageValue = damageValue;
        if (DebugMode)
            Debug.Log("On Hit Effects Triggered");
        foreach(Item item in onHitItems.Keys)
        {
            if (DebugMode)
                Debug.Log($"ItemManager::OnHit - Triggering {item}");
            modifiedDamageValue = item.OnHitEffect(modifiedDamageValue);
        }

        return modifiedDamageValue;
    }

    void AddItemToDictionaries(Item item)
    {
        switch (item.ItemType)
        {
            case ItemTypes.Instant:
                if (DebugMode)
                    Debug.Log("ItemManager::AddItemToDictionaries - Adding to instant items");
                CheckAndAdd(item, instantItems);
                break;
            case ItemTypes.OnHit:
                if (DebugMode)
                    Debug.Log("ItemManager::AddItemToDictionaries - Adding to on hit items");
                CheckAndAdd(item, onHitItems);
                break;
            default:
                Debug.LogError($"ItemManager::AddItemToDictionaries - Not yet implemented item type {item.ItemType}");
                break;
        }
    }

    void RemoveItemFromDictionaries(Item item)
    {
        switch (item.ItemType)
        {
            case ItemTypes.Instant:
                if (DebugMode)
                    Debug.Log("ItemManager::RemoveItemFromDictionaries - Removing from instant items");
                HandleRemovingItem(item, instantItems);
                break;
            case ItemTypes.OnHit:
                if (DebugMode)
                    Debug.Log("ItemManager::RemoveItemFromDictionaries - Removing from on hit items");
                HandleRemovingItem(item, onHitItems);
                break;
            default:
                Debug.LogError($"ItemManager::RemoveItemFromDictionaries - Not yet implemented item type {item.ItemType}");
                break;
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
}
