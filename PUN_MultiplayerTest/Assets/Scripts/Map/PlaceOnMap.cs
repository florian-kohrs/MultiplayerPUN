using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceOnMap : MonoBehaviour
{

    public BaseMap map;

    public int activeObjectIndex;

    public GameObject previewObject;

    protected int activeRotation;

    protected Action onDone;

    protected List<MapOccupationObject> AllMapOccupations => map.randomReceivableMapOccupationObjects;

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
        GameManager.InputHandler.input.PlayerActions.RotateObject.performed +=
            input =>
            {
                if (enabled)
                    ActiveRotation += (int)input.ReadValue<float>();
            };
    }

    //protected override void OnStart()
    //{
    //    BeginPlace(activeObject);
    //    GameManager.InputHandler.input.PlayerActions.RotateObject.performed += (val) => { ActiveRotation += (int)val.ReadValue<float>(); };
    //}

    public void BeginPlace(int mapObjectIndex, Action onDone)
    {
        ActiveRotation = 0;
        this.onDone = onDone;
        RemovePreview();
        activeObjectIndex = mapObjectIndex;
        previewObject = NetworkInstantiation.Instantiate(AllMapOccupations[activeObjectIndex].prefab);
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
        if (activeObjectIndex < 0)
            return;


        UpdateObjectPreview();

        Vector3 worldSpace = GetMouseWorldSpace();
        Vector2Int mapIndex = map.PositionToIndex(worldSpace);

        if (Mouse.current.leftButton.isPressed)
        {
            if (map.PlaceDuringRounds(activeObjectIndex, mapIndex.x, mapIndex.y, activeRotation))
            {
                RemovePreview();
                enabled = false;
                onDone();
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
        MapOccupationObject activeObject = AllMapOccupations[activeObjectIndex];

        if(activeObject.mirrorInsteadOfRotate)
        {
            Vector3 localScale = previewObject.transform.localScale;
            previewObject.transform.localScale = new Vector3(Mathf.Pow(-1, ActiveRotation) * Mathf.Abs(localScale.x), localScale.y, localScale.z);
        }
        else
        {
            previewObject.transform.rotation = map.GetObjectRotation(activeRotation);
        }

    }


    protected Vector3 GetMouseWorldSpace()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = Camera.main.nearClipPlane;
        Vector3 worldSpace = Camera.main.ScreenToWorldPoint(mousePos);
        return worldSpace + new Vector3(BaseMap.HALF_SPACING, BaseMap.HALF_SPACING,0);
    }

}
