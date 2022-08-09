using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SelectItemToPlace : MonoBehaviourPun
{

    public const int TIME_TO_SELECT_ITEM = 30;

    public PlaceOnMap placeOnMap;

    public BaseMap baseMap;

    protected List<int> availableRotation;

    protected List<MapOccupationObject> MapOccupationObjects => baseMap.randomReceivableMapOccupationObjects.MapOccupations;

    public int NumberItems { get { if (PhotonNetwork.IsConnected) return GameCycle.NumberPlayers + 2; else return 10; } }

    protected LayerMask layerMask;

    protected List<SelectItemButton> buttons;

    public GameObject itemSelectionPrefab;

    public Transform itemSelectionParent;

    public Vector2Int startPosition;
    public Vector2Int itemOffset;

    protected Transform player;

    public GameObject selectionUIParent;

    public Image selectedItemImage;

    public TimeRestricedTask timeRestriction;

    protected float currentTime;

    protected bool selected = false;

    private void Start()
    {
        selectionUIParent.SetActive(false);
        enabled = false;
    }

    protected Action<int> objectSelectedCallback;

    public void StartSelection(Action<int> onDone, System.Random rand)
    {
        //onDone(MapOccupationObjects.RandomIndex());
        enabled = true;
        currentTime = 0;
        selected = false;
        selectionUIParent.SetActive(true);
        selectedItemImage.sprite = null;
        selectedItemImage.gameObject.SetActive(false);
        timeRestriction.StartTask(TIME_TO_SELECT_ITEM, SelectRandomItem, "Select item", "Wait for other players");
        player = PlayerState.GetLocalPlayerTransform();
        PlayerState.GetLocalPlayer().body.simulated = false;
        GameCycle.IterateOverPlayers(p => p.GetActualPlayer().localScale *= 5);
        objectSelectedCallback = onDone;
        DetermineNextRotation(rand);
    }

    protected void SetPlayerToMousePosition()
    {
        player.transform.position = PlaceOnMap.GetMouseWorldSpace() + new Vector3(0,0,6);
    }

    public void DestroyButton(int index)
    {
        if (index >= buttons.Count || buttons[index] == null)
            return;

        Destroy(buttons[index].gameObject);
        buttons[index] = null;
    }

    public void ClearUpOnAllSelected()
    {
        DestroyAllButtons();
        selectionUIParent.SetActive(false);
        enabled = false;
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

    protected void SelectRandomItem()
    {
        SelectItemButton b = GetRandomAvailableItemButton();
        PlayerSelectedItem(b.ObjectIndex, b.Index);
    }

    protected SelectItemButton GetRandomAvailableItemButton()
    {
        List<SelectItemButton> availableButtons = new List<SelectItemButton>();
        foreach (SelectItemButton button in buttons)
        {
            if (button != null && button.canBeClicked)
                availableButtons.Add(button);
        }
        return availableButtons.TakeRandom();
    }

    protected void PlayerSelectedItem(int objectIndex, int btnIndex)
    {
        selected = true;
        timeRestriction.TaskDone();
        selectedItemImage.sprite = placeOnMap.AllMapOccupations[objectIndex].image; 
        selectedItemImage.gameObject.SetActive(true);
        Broadcast.SafeRPC(photonView, nameof(DestroySelectionButton), RpcTarget.All, ()=>DestroySelectionButton(btnIndex), btnIndex);
        foreach (SelectItemButton button in buttons)
        {
            if(button != null)
                button.canBeClicked = false;
        }
        objectSelectedCallback(objectIndex);
    }


    [PunRPC]
    protected void DestroySelectionButton(int index)
    {
        DestroyButton(index);
    }


    protected void PositionImage(RectTransform t, int index)
    {
        Vector2Int pos = startPosition + itemOffset * index;
        t.localPosition = new Vector3(pos.x,pos.y,0);
    }

    private void Update()
    {
        SetPlayerToMousePosition();
      
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
