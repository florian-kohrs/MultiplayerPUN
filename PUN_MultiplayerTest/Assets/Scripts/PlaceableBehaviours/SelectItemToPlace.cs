using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectItemToPlace : MonoBehaviourPun
{

    public PlaceOnMap placeOnMap;

    public BaseMap baseMap;

    protected List<int> availableRotation;

    protected List<MapOccupationObject> MapOccupationObjects => baseMap.randomReceivableMapOccupationObjects;

    public int NumberItems => GameCycle.NumberPlayers + 2;

    protected LayerMask layerMask;

    protected List<SelectItemButton> buttons;

    public GameObject itemSelectionPrefab;

    public Transform itemSelectionParent;

    public Vector2Int startPosition;
    public Vector2Int itemOffset;


    private void Start()
    {
        //StartSelection(null, new System.Random());
        //DetermineNextRotation();
    }

    protected Action<int> objectSelectedCallback;

    public void StartSelection(Action<int> onDone, System.Random rand)
    {
            //onDone(MapOccupationObjects.RandomIndex());
        objectSelectedCallback = onDone;
        DetermineNextRotation(rand);
    }

    public void DestroyButton(int index)
    {
        if (index >= buttons.Count || buttons[index] == null)
            return;

        Destroy(buttons[index].gameObject);
        buttons[index] = null;
    }

    public void DestroyAllButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            DestroyButton(i);
        }
        buttons.Clear();
    }

    public void DetermineNextRotation(System.Random rand)
    {
        buttons = new List<SelectItemButton>();
        availableRotation = new List<int>(NumberItems);
        for (int i = 0; i < NumberItems; i++)
        {
            availableRotation.Add(MapOccupationObjects.RandomIndex(rand));
            Display(availableRotation[i], i);
        }
        enabled = true;
    }

    protected void ClearChildren()
    {
        while(transform.childCount> 0)
        {
            Destroy(transform.GetChild(0));
        }
    }

    public GameObject Display(int objectIndex, int index)
    {
        GameObject g = Instantiate(itemSelectionPrefab, itemSelectionParent);
        SelectItemButton button = g.GetComponent<SelectItemButton>();
        buttons.Add(button);
        button.CreateItemSelection(index, MapOccupationObjects[objectIndex], objectIndex, PlayerSelectedItem);
        RectTransform rectTransform = g.GetComponent<RectTransform>();
        PositionImage(rectTransform,index);
        return g;
    }

    protected void PlayerSelectedItem(int index)
    {
        Broadcast.SafeRPC(photonView, nameof(DestroySeletionButton), RpcTarget.All, ()=>DestroySeletionButton(index), index);
        foreach (SelectItemButton button in buttons)
        {
            if(button != null)
                button.canBeClicked = false;
        }
        objectSelectedCallback(index);
    }


    [PunRPC]
    protected void DestroySeletionButton(int index)
    {
        DestroyButton(index);
    }


protected void PositionImage(RectTransform t, int index)
    {
        Vector2Int pos = startPosition + itemOffset * index;
        t.localPosition = new Vector3(pos.x,pos.y,0);
    }

    //private void Update()
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
    //    Ray2D ray2D = new Ray2D(ray.origin, ray.direction);
    //    RaycastHit2D hit = Physics2D.Raycast(ray2D.origin, ray2D.direction, 100, layerMask);
    //    if (hit.point != default)
    //    {
    //        Debug.Log(hit.transform.name);
    //        Debug.Log("hit");
    //        if(Mouse.current.leftButton.isPressed)
    //        {
    //            Debug.Log("Selected" + hit.transform.name);
    //            ClearChildren();
    //        }
    //    }
    //}
}
