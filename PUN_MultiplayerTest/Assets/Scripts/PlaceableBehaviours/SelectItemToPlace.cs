using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectItemToPlace : MonoBehaviour
{

    public PlaceOnMap placeOnMap;

    public List<MapOccupationObject> allOccupationObjects;

    protected List<MapOccupationObject> availableRotation;

    protected List<GameObject> allItemSlots;

    public GameObject itemSlotPrefab;

    public const int NUMBER_ITEMS = 4;

    public void DetermineNextRotation()
    {

        availableRotation = new List<MapOccupationObject>(NUMBER_ITEMS);
        allItemSlots = new List<GameObject>(NUMBER_ITEMS);
        for (int i = 0; i < NUMBER_ITEMS; i++)
        {
            availableRotation[i] = allOccupationObjects.GrabOne();
        }
        ItemDisplayer.Fill(new Rect(new Vector2(0, 0), new Vector2(100, 100)),slotP);
    }

    protected void ClearChildren()
    {
        while(transform.childCount> 0)
        {
            Destroy(transform.GetChild(0));
        }
    }

    public GameObject Display(MapOccupationObject mapObject, int index, Vector2 pos)
    {
        GameObject itemInstance = Instantiate(mapObject.prefab, transform);
        InventorySlot slot = itemInstance.GetO
    }
}
