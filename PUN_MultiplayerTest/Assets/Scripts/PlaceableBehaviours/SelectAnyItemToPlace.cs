using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SelectAnyItemToPlace : MonoBehaviour
{

    public BaseMap map;

    protected List<MapOccupationObject> MapOccupationObjects => map.AllMapOccupationObjects;

    public GameObject itemButtonPrefab;

    public Image currentSelectedItemUI;

    public PlaceOnMap placer;

    public GridLayoutGroup grid;

    private void Update()
    {
        if(Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            grid.gameObject.SetActive(!grid.gameObject.activeSelf);
        }
    }

    private void Start()
    {
        for (int i = 0; i < MapOccupationObjects.Count; i++)
        {
            Display(i, i);
        }
    }

    public GameObject Display(int objectIndex, int index)
    {
        GameObject g = Instantiate(itemButtonPrefab, grid.transform);
        SelectItemButton button = g.GetComponent<SelectItemButton>();
        button.CreateItemSelection(index, MapOccupationObjects[objectIndex], objectIndex, SelectedItem);
        return g;
    }

    protected void SelectedItem(int index)
    {
        currentSelectedItemUI.sprite = MapOccupationObjects[index].image;
        placer.BeginPlace(index, null);
    }

}
