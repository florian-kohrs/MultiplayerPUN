using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapOccupationObject : ScriptableObject
{

    private void OnValidate()
    {
        if (localOccupationSpots.Contains(new Vector2Int(0, 0)))
        {
            localOccupationSpots.Remove(new Vector2Int(0, 0));
        }
    }

    public bool CanBePlacedWithFieldOccupied(MapOccupation currentOccupier, Vector2Int placePose, int rotation, BaseMap map)
    {

        bool isFieldOccupied = currentOccupier != null;

        if (isBomb)
            return true;

        ///return true if the target field is free and this object should not be placed on another object
        if (!isFieldOccupied && !mustBePlacedOnOtherObject)
            return true;

        bool result = mustBePlacedOnOtherObject && currentOccupier != null;

        if (isFieldOccupied && mustBePlacedOnOtherObject)
        {
            MapOccupation rotatedNeighbour;
            Vector2Int neighbourPos = placePose + RotateOccupation(new Vector2Int(0, 1), rotation);
            result &= map.IsInBounds(neighbourPos);

            if (map.TryGetOccupationAt(neighbourPos, out rotatedNeighbour))
            {
                ///neighbour has to be different than the object it is placed on
                result &= currentOccupier != rotatedNeighbour;
                ///if neighbour exists it must be able to be glueable
                result &= rotatedNeighbour.CanBeGlued;
            }
        }
        return result;
    }

    public bool canBeGlued;

    public bool mustBePlacedOnOtherObject;

    public Sprite image;

    public bool occupiesDefaultPosition = true;

    public bool isBomb;
    public enum OccupationType { Obstacle, Danger, Destruction}

    public OccupationType type;

    public GameObject prefab;

    public bool mirrorInsteadOfRotate = false;

    [Tooltip("Dont add default value (0,0) here")]
    public List<Vector2Int> localOccupationSpots;

    public IEnumerable<Vector2Int> LocalOccupationSpots()
    {
        if(occupiesDefaultPosition)
            yield return new Vector2Int(0, 0);
        foreach(var occ in localOccupationSpots)
            yield return occ;
    }

    public int OccupationCount 
    {
        get 
        {
            int count = localOccupationSpots.Count;
            if (occupiesDefaultPosition)
                count++;
            return Mathf.Max(1, count);
        }
    }

    public IEnumerable<Vector2Int> GetAllOccupations(Vector2Int origin, int orientation)
    {
        foreach (var item in LocalOccupationSpots())
        {
            if (mirrorInsteadOfRotate)
            {
                yield return origin + MirrorOccupation(item, orientation);
            }
            else
            {
                yield return origin + RotateOccupation(item, orientation);
            }
        }
    }

    protected Vector2Int MirrorOccupation(Vector2Int relativePos, int mirrored)
    {
        mirrored = mirrored % 2;
        Vector2Int relative = relativePos;
        if (mirrored == 1)
            relative = new Vector2Int(-relative.x, relative.y);
        return relative;
    }

    protected Vector2Int RotateOccupation(Vector2Int relativePos, int orientation)
    {
        orientation = orientation % 4;
        Vector2Int relative = relativePos;
        if (orientation == 1)
            relative = new Vector2Int(relative.y, -relative.x);
        else if (orientation == 2)
            relative = new Vector2Int(-relative.x, -relative.y);
        else if (orientation == 3)
            relative = new Vector2Int(-relative.y, relative.x);
        return relative;
    }

    public virtual void ApplyToObject(GameObject prefabInstace) { }

}
