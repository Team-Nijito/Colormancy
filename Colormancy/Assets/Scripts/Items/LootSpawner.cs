using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using System.Linq;
using UnityEditor;

public class LootSpawner : MonoBehaviour
{
    string pathToItems = "GameItems";

    MMLootTable<MMLoot<GameObject>, GameObject> lootTable = new MMLootTable<MMLoot<GameObject>, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Loading Loot...");
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
            var gameItems = Resources.LoadAll<MonoScript>(pathToItems);
            foreach(var script in gameItems)
            {
                MMLoot<GameObject> lootItem = new MMLoot<GameObject>();
                var i = script.GetClass();
                GameObject lootObject = new GameObject();
                lootObject.AddComponent(i);
                Item item = lootObject.GetComponent<Item>();
                lootItem.ChancePercentage = item.GetWeight();
                lootItem.Loot = lootObject;
                lootTable.ObjectsToLoot.Add(lootItem);
            }
        }

        //Debug.Log(lootTable.GetLoot().Loot.name);
    }
}
