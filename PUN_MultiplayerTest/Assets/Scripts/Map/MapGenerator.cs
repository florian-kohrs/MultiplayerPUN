using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    protected int floorObjectIndex = 2;

    public MapOccupationObject finishObject;

    public List<MapDesign> maps;

    public int startAreaIndex = 0;
    public int finishAreaIndex = 1;

    public BaseMap map;

    public void StartMap(int index)
    {
        if(index < 0)
            GenerateGarbage();
        else 
            map.LoadMapDesign(maps[index]);
    }

    protected void GenerateGarbage()
    {
        for (int i = 0; i < map.Dimensions.x; i++)
        {
            map.PlaceGeneration(floorObjectIndex, i, 0, 0);
        }
        SetStart();
        SetFinish();
    }

    protected void SetStart()
    {
        map.SetStartPoint(DetermineStartPoint(), startAreaIndex);
    }

    protected void SetFinish()
    {
        Vector2Int finishPoint = DetermineFinishPoint();
        if (!map.PlaceGeneration(finishAreaIndex, finishPoint.x, finishPoint.y, 0))
            throw new System.Exception("End Object must be able to Spawn!");
    }


    protected Vector2Int DetermineStartPoint()
    {
        return new Vector2Int(3, 1);
    }

    protected Vector2Int DetermineFinishPoint()
    {
        return new Vector2Int(map.Dimensions.x - 4, 1);
    }

}
