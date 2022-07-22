using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BaseMap : MonoBehaviour
{

    public const float MAP_FIELD_SPACING = 1;

    public Vector3 Anchor => transform.position;

    public Vector2 Anchor2D => new Vector2(transform.position.x, transform.position.y);

    public Vector2Int dimensions;

    public MapOccupation[,] occupationMap;

    private void Awake()
    {
        occupationMap = new MapOccupation[dimensions.x,dimensions.y];
    }

    public bool Place(MapOccupationObject occupation, Vector2Int origin, int rotation)
    {
        bool result = IsInBounds(origin) && IsSpaceNotOccupied(occupation,origin,rotation);
        if(result)
        {
            MapOccupation mapOccupation = new MapOccupation(occupation,origin,rotation);
            OccupySpace(mapOccupation);
        }
        return result;
    }

    protected bool IsInBounds(Vector2Int pos)
    {
        return pos.x > 0 && pos.y > 0 && pos.x < dimensions.x && pos.y < dimensions.y;
    }


    public Vector2Int PositionToIndex(Vector2 pos)
    {
        Vector2 localPos = pos - Anchor2D;
        Vector2Int index = new Vector2Int(
           Mathf.FloorToInt(localPos.x / MAP_FIELD_SPACING),
           Mathf.FloorToInt(localPos.y / MAP_FIELD_SPACING));
        return index;
    }

    protected bool IsSpaceNotOccupied(MapOccupationObject occupation, Vector2Int origin, int rotation)
    {
        bool result = true;
        foreach(var spot in occupation.GetAllOccupations(origin,rotation))
        {
            if(occupationMap[spot.x, spot.y] != null)
            {
                result = false;
                break;
            }
        }
        return result;
    }

    protected void OccupySpace(MapOccupation occupation)
    {
        foreach (var spot in occupation.GetAllOccupations())
        {
            occupationMap[spot.x, spot.y] = occupation;
        }
    }

}

