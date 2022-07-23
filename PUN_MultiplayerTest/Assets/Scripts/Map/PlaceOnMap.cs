using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceOnMap : MonoBehaviour
{

    public BaseMap map;

    public MapOccupationObject activeObject;

    public GameObject previewObject;

    protected int activeRotation;

    protected int ActiveRotation
    {
        get { return activeRotation; }
        set
        {
            activeRotation = value % 4;
        }
    }

    private void Start()
    {
        enabled = false;
    }

    //protected override void OnStart()
    //{
    //    BeginPlace(activeObject);
    //    GameManager.InputHandler.input.PlayerActions.RotateObject.performed += (val) => { ActiveRotation += (int)val.ReadValue<float>(); };
    //}

    protected void BeginPlace(MapOccupationObject mapObject)
    {
        RemovePreview();
        activeObject = mapObject;
        previewObject = Instantiate(mapObject.prefab);
        previewObject.GetComponentsInChildren<Behaviour>().ToList().ForEach((b) => b.enabled = false);
        UpdateObjectPreview();
        enabled = true;
    }

    protected void RemovePreview()
    {
        if (previewObject != null) 
        {
            Destroy(previewObject);
            previewObject = null;
        }
    }

    void Update()
    {
        if (activeObject == null)
            return;


        UpdateObjectPreview();

        Vector3 worldSpace = GetMouseWorldSpace();
        Vector2Int mapIndex = map.PositionToIndex(worldSpace);

        if (Mouse.current.leftButton.isPressed)
        {
            if (map.Place(activeObject, mapIndex, activeRotation))
            {
                RemovePreview();
                enabled = false;
            }
        }
    }


    protected Vector3 GlobalPositionFromMouse()
    {
        Vector3 worldSpace = GetMouseWorldSpace();
        Vector2Int mapIndex = map.PositionToIndex(worldSpace);
        return map.MapIndexToGlobalPosition(mapIndex);
    }

    protected void UpdateObjectPreview()
    {
        previewObject.transform.position = GlobalPositionFromMouse();
        previewObject.transform.rotation = map.GetObjectRotation(activeRotation);
    }


    protected Vector3 GetMouseWorldSpace()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = Camera.main.nearClipPlane;
        Vector3 worldSpace = Camera.main.ScreenToWorldPoint(mousePos);
        return worldSpace + new Vector3(BaseMap.HALF_SPACING, BaseMap.HALF_SPACING,0);
    }

}
