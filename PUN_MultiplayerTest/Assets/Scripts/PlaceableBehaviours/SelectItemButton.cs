using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectItemButton : MonoBehaviourPun
{

    protected Action<int> callback;
    protected int objectIndex;
    protected MapOccupationObject mapObject;
    protected int index;
    protected SelectItemToPlace itemSelector;

    public bool canBeClicked = true;

    public void CreateItemSelection(SelectItemToPlace itemSelector, int index, MapOccupationObject occupation, int occupationObjectIndex, Action<int> onItemSelected)
    {
        this.itemSelector = itemSelector;
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
            callback(objectIndex);
            Broadcast.SafeRPC(photonView, nameof(Destroy), RpcTarget.All, Destroy);
        }
    }

    [PunRPC]
    protected void Destroy()
    {
        itemSelector.DestroyButton(index);
    }

}
