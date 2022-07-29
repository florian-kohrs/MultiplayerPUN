using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapOccupationList : ScriptableObject
{

    public List<MapOccupationObject> MapOccupations
    {
        get
        {
            return mapOccupationsEnumerable.ToList();
        }
    }

    public IEnumerable<MapOccupationObject> mapOccupationsEnumerable
    {
        get
        {
            foreach (var item in mapOccupations)
            {
                yield return item;
            }

            foreach (var items in includedLists)
            {
                foreach (var item in items.MapOccupations)
                {
                    yield return item;
                }
            }
        }
    }

    [SerializeField]
    public List<MapOccupationObject> mapOccupations;

    [SerializeField]
    public List<MapOccupationList> includedLists;

}
