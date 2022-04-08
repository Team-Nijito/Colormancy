using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using System.Linq;
using UnityEditor;

public class LootSpawner : MonoBehaviour
{
    string pathToItems = "Items";

    MMLootTable<MMLoot<ItemSO>, ItemSO> lootTable;

    // Start is called before the first frame update
    void Start()
    {
        CreateAndFillLootTable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateAndFillLootTable()
    {
        if (lootTable == null)
        {
            lootTable = new MMLootTable<MMLoot<ItemSO>, ItemSO>();
            lootTable.ObjectsToLoot = new List<MMLoot<ItemSO>>();

            ItemSO[] gameItems = Resources.LoadAll<ItemSO>(pathToItems);
            foreach(ItemSO itemSO in gameItems)
            {
                MMLoot<ItemSO> lootItem = new MMLoot<ItemSO>();
                lootItem.ChancePercentage = itemSO.itemWeight;
                lootItem.Loot = itemSO;
                lootTable.ObjectsToLoot.Add(lootItem);
            }
        }

        Debug.Log($"Filled loot table with {lootTable.ObjectsToLoot.Count} item(s)");
    }
}
