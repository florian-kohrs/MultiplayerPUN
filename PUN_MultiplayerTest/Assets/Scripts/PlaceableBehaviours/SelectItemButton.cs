using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectItemButton : MonoBehaviour
{

    protected Action<int, int> callback;
    protected int objectIndex;
    protected MapOccupationObject mapObject;
    protected int index;

    public bool canBeClicked = true;

    public void CreateItemSelection(int index, MapOccupationObject occupation, int occupationObjectIndex, Action<int, int> onItemSelected)
    {
        callback = onItemSelected;
        mapObject = occupation; 
        GetComponent<Image>().sprite = occupation.image;
        this.objectIndex = occupationObjectIndex;
        this.index = index;
    }

    public void ButtonClicked()
    {
        if (canBeClicked)
        {
            callback(objectIndex, index);
        }
    }

}
