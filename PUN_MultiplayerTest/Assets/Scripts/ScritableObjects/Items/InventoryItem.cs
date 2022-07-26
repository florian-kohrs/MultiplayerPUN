using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem : ScriptableObject
{
    
    public string itemName;

    public ItemType itemType;

    public enum ItemType { Hat, Hair, Beard, Other}
    
}
