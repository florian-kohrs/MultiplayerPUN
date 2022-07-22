using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceOnMap : PunLocalBehaviour
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

        Vector3 worldSpace = GetCameraWorldSpace();
        Vector2Int mapIndex = map.PositionToIndex(worldSpace);

        if (Mouse.current.leftButton.isPressed)
        {
            if (map.Place(activeObject, mapIndex, activeRotation))
            {
                Spawn(MapIndexToGlobalPosition(mapIndex));
                RemovePreview();
                enabled = false;
            }
        }
    }

    protected void Spawn(Vector3 pos)
    {
        Quaternion activeRotation = GetObjectRotation();
        NetworkInstantiation.Instantiate(activeObject.prefab, pos, activeRotation);
    }

    protected Vector3 GlobalPositionFromMouse()
    {
        Vector3 worldSpace = GetCameraWorldSpace();
        Vector2Int mapIndex = map.PositionToIndex(worldSpace);
        return MapIndexToGlobalPosition(mapIndex);
    }

    protected void UpdateObjectPreview()
    {
        previewObject.transform.position = GlobalPositionFromMouse();
        previewObject.transform.rotation = GetObjectRotation();
    }

    protected Quaternion GetObjectRotation()
    {
        return Quaternion.Euler(0, 0, activeRotation * 90);
    }

    protected Vector3 MapIndexToGlobalPosition(Vector2Int index)
    {
        return map.transform.position + (index.ToVector2() * BaseMap.MAP_FIELD_SPACING).LiftVectorOnXY();
    }

    protected Vector3 GetCameraWorldSpace()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = Camera.main.nearClipPlane;
        Vector3 worldSpace = Camera.main.ScreenToWorldPoint(mousePos);
        return worldSpace;
    }
}
