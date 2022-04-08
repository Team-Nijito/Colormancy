using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using MyBox;
using UnityEngine;

[CreateAssetMenu(fileName = "Name", menuName = "Colormancy/Create Item", order = 1)]
public class ItemSO : ScriptableObject
{
    [Separator("In Game Settings")]
    public string itemName;
    public string itemDescription;
    public string itemFlavorText;

    [Separator("For Use Behind the Scenes")]
    public float itemWeight = 1f;
    public string itemScriptName;
}