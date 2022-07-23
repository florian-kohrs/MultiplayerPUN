using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapOccupationObject : ScriptableObject
{

    private void OnValidate()
    {
        if(localOccupationSpots.Contains(new Vector2Int(0,0)))
        {
            localOccupationSpots.Remove(new Vector2Int(0,0));
        }
    }

    public GameObject prefab;

    public bool destroyable = true;

    public bool mirrorInsteadOfRotate = false;

    [Tooltip("Dont add default value (0,0) here")]
    public List<Vector2Int> localOccupationSpots;

    public IEnumerable<Vector2Int> LocalOccupationSpots()
    {
        yield return new Vector2Int(0, 0);
        foreach(var occ in localOccupationSpots)
            yield return occ;
    }


    public IEnumerable<Vector2Int> GetAllOccupations(Vector2Int origin, int orientation)
    {
        foreach (var item in LocalOccupationSpots())
        {
            yield return TransformOccupation(item, origin, orientation);
        }
    }

    protected Vector2Int TransformOccupation(Vector2Int pos, Vector2Int origin, int orientation)
    {
        orientation = orientation % 4;
        Vector2Int relative = pos;
        if (orientation == 1)
            relative = new Vector2Int(relative.y, -relative.x);
        else if (orientation == 2)
            relative = new Vector2Int(-relative.x, -relative.y);
        else if (orientation == 3)
            relative = new Vector2Int(-relative.y, relative.x);
        return relative + origin;
    }

}
