using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDesigner : BaseMap
{

    public CameraMover cam;

    protected MapDesign currentMapDesign;

    public LevelSelection levelSelection;

    protected override bool BombsDestroyEverything => true;

    private void Start()
    {
        StartWithMapDesign(levelSelection.SelectedMapDesign);
        cam.SetToSpectateView();
        levelSelection.onSelectedMapChanged += LoadLevel;
    }

    public void LoadLevel(MapDesign mapDesign)
    {
        currentMapDesign = mapDesign;
        LoadMapDesign(mapDesign);
    }

    public void StartWithMapDesign(MapDesign mapDesign)
    {
        LoadLevel(mapDesign);
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

    protected override void OnDestroyedOccupation(Vector2Int origin, bool removeFromMapObject)
    {
        if(removeFromMapObject)
            currentMapDesign.DestroyAt(origin);
    }


}
