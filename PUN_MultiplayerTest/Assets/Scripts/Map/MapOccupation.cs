using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapOccupation
{

    public MapOccupation(MapOccupationObject occupationObject, Vector2Int origin, int orientation, bool destroyable = true)
    {
        this.occupationObject = occupationObject;
        this.origin = origin;
        Orientation = orientation;
    }

    public MapOccupationObject occupationObject;

    public Vector2Int origin;

    protected int orientation;

    public bool destroyable;

    public int Orientation
    {
        get { return orientation; }
        set { orientation = value % 4; }
    }

    protected IEnumerable<Vector2Int> GetAllUnrotatedOccupationSlots()
    {
        return occupationObject.LocalOccupationSpots();
    }

    public IEnumerable<Vector2Int> GetAllOccupations()
    {
        return occupationObject.GetAllOccupations(origin, orientation);
    }

}
