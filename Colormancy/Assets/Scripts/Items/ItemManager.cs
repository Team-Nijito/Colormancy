using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviourPun
{
    public List<Item> currentItems = new List<Item>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddItem(Item item)
    {
        item.Init();
        item.AddItemEffects();
        currentItems.Add(item);
    }

    public void RemoveItem(Item item)
    {
        if (currentItems.Contains(item))
        {
            item.RemoveItemEffects();
            currentItems.Remove(item);
        }
    }
}
