using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ListSelection : MonoBehaviour
{

    protected List<EquipableItemAsset> availableItems;

    [SerializeField]
    protected TextMeshProUGUI selectionText;

    protected int currentSelectedIndex = 0;

    protected Transform itemParent; 

    public int CurrentSelectedIndex
    {
        get { return currentSelectedIndex; }
    }

    public int ReducedCurrentSelectedIndex
    {
        get { return currentSelectedIndex - 1; }
    }

    protected void ChangeCurrentSelectedIndex(int delta)
    {
        currentSelectedIndex += delta;
        if (currentSelectedIndex < 0)
            currentSelectedIndex += maxItems;
        else
            currentSelectedIndex = currentSelectedIndex % maxItems; 
    }

    protected Action onDesignChanged;

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    protected int maxItems;

    protected GameObject equippedItem;
      
    public void Initialize(Vector2 localPos, string displayName, List<EquipableItemAsset> items, Transform equipParent, Action onDesignChanged)
    {
        ///add +1 to allow -1 as no selection
        maxItems = items.Count + 1;
        this.onDesignChanged = onDesignChanged;
        selectionText.text = displayName;
        availableItems = items;
        itemParent = equipParent;
        transform.localPosition = localPos;
        gameObject.SetActive(false);
    }

    public void OpenSelection()
    {
        gameObject.SetActive(true);
    }

    public void UpdateSelection(int index)
    {
        currentSelectedIndex = index;
        EquipItem();
    }

    public void ChangeItem(int sign)
    {
        ChangeCurrentSelectedIndex(sign);
        EquipItem();
        onDesignChanged();
    }

    protected void EquipItem()
    {
        ClearItem();
        int index = ReducedCurrentSelectedIndex;
        if (index >= 0)
        {
            equippedItem = availableItems[index].GetItemInstance(itemParent);
        }
    }

    protected void ClearItem()
    {
        if (equippedItem != null) 
        { 
            Destroy(equippedItem);
            equippedItem = null;
        }
    }

}
