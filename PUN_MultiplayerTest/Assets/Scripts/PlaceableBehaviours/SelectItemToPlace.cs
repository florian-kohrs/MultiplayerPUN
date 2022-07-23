using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectItemToPlace : MonoBehaviour
{

    public PlaceOnMap placeOnMap;

    public List<MapOccupationObject> allOccupationObjects;

    protected List<MapOccupationObject> availableRotation;

    public const int NUMBER_ITEMS = 4;

    protected LayerMask layerMask;

    public GameObject inventorySlot;

    private void Start()
    {
        layerMask = LayerMask.NameToLayer("ObjectSelection");
        enabled = false;
        DetermineNextRotation();
    }

    public void DetermineNextRotation()
    {
        availableRotation = new List<MapOccupationObject>(NUMBER_ITEMS);
        for (int i = 0; i < NUMBER_ITEMS; i++)
        {
            availableRotation.Add(allOccupationObjects.GrabOne());
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

    public GameObject Display(MapOccupationObject mapObject, int index)
    {
        GameObject itemInstance = Instantiate(mapObject.prefab, transform);
        OccupationObjectRef objectSelection = itemInstance.AddComponent<OccupationObjectRef>();
        objectSelection.mapObject = mapObject;
        objectSelection.index = index;
        itemInstance.layer = layerMask;

        Vector3 pos = Camera.main.transform.position;
        pos.z = 0;
        float progress = index / (float)NUMBER_ITEMS;
        int spacing = 4;
        Vector2 relativePos = new Vector2(Mathf.Sin(Mathf.PI * 2 * progress) * spacing, Mathf.Cos(Mathf.PI * 2 * progress) * spacing);
        itemInstance.transform.position = pos + new Vector3(relativePos.x, relativePos.y, 0);
        return itemInstance;
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Ray2D ray2D = new Ray2D(ray.origin, ray.direction);
        RaycastHit2D hit = Physics2D.Raycast(ray2D.origin, ray2D.direction, 100, layerMask);
        if (hit.point != default)
        {
            Debug.Log(hit.transform.name);
            Debug.Log("hit");
            if(Mouse.current.leftButton.isPressed)
            {
                Debug.Log("Selected" + hit.transform.name);
                ClearChildren();
            }
        }
    }
}
