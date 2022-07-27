using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDesigner : BaseMap
{

    public CameraMover cam;

    public MapDesign currentMapDesign;

    public Transform mapMarker;

    protected override bool BombsDestroyEverything => true;

    private void Start()
    {
        StartWithMapDesign(currentMapDesign);
        cam.SetToSpectateView();
    }

    public void StartWithMapDesign(MapDesign mapDesign)
    {
        currentMapDesign = mapDesign;
        LoadMapDesign(mapDesign);
        mapMarker.position = (mapDesign.dimensions / 2).LiftVectorOnXY();
        mapMarker.localScale = (mapDesign.dimensions).LiftVectorOnXY();
    }


    public bool PlaceForMapDesign(int occupationIndex, int originX, int originY, int rotationIndex)
    {
        bool result = PlaceGeneration(occupationIndex, originX, originY, rotationIndex);
        if (!result)
            return false;

        MapOccupation occupation = OccupationMap[originX, originY];
        ///if the occupation is null the placed object was a bomb
        if(occupation != null) 
        { 
            currentMapDesign.Place(new Vector2Int(originX, originY), occupation);
        }
        return true;

    }

    protected override void OnDestroyedOccupation(Vector2Int origin)
    {
        currentMapDesign.DestroyAt(origin);
    }


}
