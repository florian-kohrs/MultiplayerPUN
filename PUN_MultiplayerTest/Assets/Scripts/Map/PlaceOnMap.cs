using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceOnMap : MonoBehaviour
{

    protected CameraMover camMover;

    public CameraMover CamMover
    {
        get
        {
            if (camMover == null)
                camMover = GameManager.GetPlayerComponent<CameraMover>();
            return camMover;
        }
    }

    public static readonly Color CAN_PLACE_COLOR = Color.green;

    public static readonly Color CAN_NOT_PLACE_COLOR = Color.red;

    public BaseMap map;

    public int activeObjectIndex;

    protected GameObject previewObject;

    protected int activeRotation;

    protected Action onDone;

    public bool useAllItems;

    public GameObject spriteMarker;

    protected List<SpriteRenderer> activeSprites = new List<SpriteRenderer>();

    protected MapOccupationObject SelectedMapOccupation => AllMapOccupations[activeObjectIndex];

    public TimeRestricedTask timeRestriction;

    public List<MapOccupationObject> AllMapOccupations
    {
        get
        {
            if (useAllItems)
                return map.AllMapOccupationObjects;
            else
                return map.randomReceivableMapOccupationObjects.MapOccupations;
        }
    }

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
        previewObject = Instantiate(SelectedMapOccupation.prefab);
        AllMapOccupations[activeObjectIndex].ApplyToObject(previewObject);
        previewObject.GetComponentsInChildren<Behaviour>().ToList().ForEach((b) => b.enabled = false);
        UpdateObjectPreview();
        CreateMarkersForObject();
        timeRestriction.StartTask(TimeToPlace, DiscardSelection, "Place item on map", "Wait for other players");
        enabled = true;
    }

    protected virtual float TimeToPlace
    {
        get
        {
            if (map is MapDesigner)
                return Mathf.Infinity;
            else
                return 20;
        }
    }

    protected void RemovePreview()
    {
        if (previewObject != null) 
        {
            Destroy(previewObject);
            previewObject = null;
        }
    }

    protected void ClearPreviewMarkers()
    {
        foreach (var sprite in activeSprites)
            Destroy(sprite.gameObject);
        activeSprites.Clear();
    }

    protected void CreateMarkersForObject()
    {
        ClearPreviewMarkers();
        for(int i = 0; i < SelectedMapOccupation.OccupationCount; i++)
        {
            GameObject g = Instantiate(spriteMarker);
            activeSprites.Add(g.GetComponent<SpriteRenderer>());
        }
    }

    protected Color QuarterAlpha(Color c)
    {
        Color newC = c;
        newC.a /= 4;
        return newC;
    }

    protected void UpdatePreviewMarker(Vector2Int origin)
    {
        int count = 0;
        foreach (var spot in SelectedMapOccupation.GetAllOccupations(origin, activeRotation))
        {
            SpriteRenderer r = activeSprites[count];
            r.color = QuarterAlpha(GetColorForPlacingAt(spot));
            r.transform.position = GetCenterFor(spot);
            count++;
        }
    }

    protected Vector3 GetCenterFor(Vector2Int index)
    {
        return map.MapIndexToGlobalPosition(index)/* + new Vector3(0.5f,0.5f,0)*/;
    }

    protected Color GetColorForPlacingAt(Vector2Int index)
    {
        if (map.IsSpaceFeasible(index))
            return CAN_PLACE_COLOR;
        else
            return CAN_NOT_PLACE_COLOR;
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
            if(map is MapDesigner m)
            {
                m.PlaceForMapDesign(activeObjectIndex, mapIndex.x, mapIndex.y, activeRotation);
            }
            else if (map.PlaceDuringRounds(activeObjectIndex, mapIndex.x, mapIndex.y, activeRotation))
            {
                RemovePreview();
                ClearPreviewMarkers();
                enabled = false;
                onDone?.Invoke();
            }
        }
        else
        {
            UpdatePreviewMarker(mapIndex);
        }
    }

    protected void DiscardSelection()
    {
        RemovePreview();
        ClearPreviewMarkers();
        enabled = false;
        onDone?.Invoke();
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
        if(PlayerState.HasPlayer)
            PlayerState.GetLocalPlayerTransform().position = previewObject.transform.position + new Vector3(3,3,6);

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


    public static Vector3 GetMouseWorldSpace()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = Camera.main.nearClipPlane;
        Vector3 worldSpace = Camera.main.ScreenToWorldPoint(mousePos);
        return worldSpace + new Vector3(BaseMap.HALF_SPACING, BaseMap.HALF_SPACING,0);
    }

}
