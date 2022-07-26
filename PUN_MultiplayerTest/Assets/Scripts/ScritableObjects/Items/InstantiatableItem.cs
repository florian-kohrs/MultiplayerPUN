using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatableItem : InventoryItem, IInstantiableItem
{
    
    public GameObject prefab;


    public virtual GameObject GetItemInstance(Transform parent)
    {
        GameObject result = Instantiate(prefab, parent);
        result.transform.localPosition = Vector3.zero;
        return result;
    }
}
