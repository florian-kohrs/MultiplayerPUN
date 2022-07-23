using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BaseMap : MonoBehaviour
{

    public const float MAP_FIELD_SPACING = 1;
    public const float HALF_SPACING = MAP_FIELD_SPACING / 2;

    public Vector3 Anchor => transform.position;

    public Vector2 Anchor2D => new Vector2(transform.position.x, transform.position.y);

    public Vector2Int dimensions;

    public MapOccupation[,] occupationMap;

    public Vector2Int startPoint;

    protected readonly Vector3 PlayerOffset = new Vector3(0, 0.5f, 0);

    public void SetStartPoint(Vector2Int start, MapOccupationObject startArea)
    {
        startPoint = start;
        if (!Place(startArea, start, 0, false))
            throw new Exception("Start Area must be able to spawn!");

    }

    public void PositionPlayers(List<Player> players)
    {
        foreach (Player p in players)
            p.transform.position = MapIndexToGlobalPosition(startPoint) + PlayerOffset;
    }

    private void Awake()
    {
        occupationMap = new MapOccupation[dimensions.x,dimensions.y];
    }

    public bool Place(MapOccupationObject occupation, Vector2Int origin, int rotationIndex, bool destroyable = true)
    {
        bool result = IsInBounds(origin) && IsSpaceNotOccupied(occupation,origin,rotationIndex);
        if(result)
        {
            MapOccupation mapOccupation = new MapOccupation(occupation,origin,rotationIndex, destroyable);
            OccupySpace(mapOccupation);
            Quaternion rotation = Quaternion.identity;
            Vector3 scale = Vector3.one;

            if(occupation.mirrorInsteadOfRotate)
                scale = new Vector3(Mathf.Pow(-1, rotationIndex), 1, 1);
            else
                rotation = GetObjectRotation(rotationIndex);

            Spawn(occupation, MapIndexToGlobalPosition(origin), rotation, scale);
        }
        return result;
    }


    protected void Spawn(MapOccupationObject occupation, Vector3 pos, Quaternion rotation, Vector3 scale)
    {
        NetworkInstantiation.Instantiate(occupation.prefab, pos, rotation, scale);
    }


    public Vector3 MapIndexToGlobalPosition(Vector2Int index)
    {
        return transform.position + (index.ToVector2() * MAP_FIELD_SPACING).LiftVectorOnXY();
    }


    public Quaternion GetObjectRotation(int rotation)
    {
        return Quaternion.Euler(0, 0, rotation * 90);
    }

    protected bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < dimensions.x && pos.y < dimensions.y;
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

