using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    public MapOccupationObject floorObject;

    public MapOccupationObject finishObject;

    public MapOccupationObject startArea;

    public BaseMap map;

    public void Generate()
    {
        for (int i = 0; i < map.dimensions.x; i++)
        {
            map.Place(floorObject, new Vector2Int(i, 0), 0, false);
        }
        SetStart();
        SetFinish();
    }

    protected void SetStart()
    {
        map.SetStartPoint(DetermineStartPoint(), startArea);
    }

    protected void SetFinish()
    {
        if (!map.Place(finishObject, DetermineFinishPoint(), 0))
            throw new System.Exception("End Object must be able to Spawn!");
    }


    protected Vector2Int DetermineStartPoint()
    {
        return new Vector2Int(2, 1);
    }

    protected Vector2Int DetermineFinishPoint()
    {
        return new Vector2Int(map.dimensions.x - 3, 1);
    }

}
