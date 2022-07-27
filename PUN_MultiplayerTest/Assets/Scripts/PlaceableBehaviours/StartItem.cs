using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartItem : BasePlaceableBehaviours
{

    public override void OnPlace(BaseMap map)
    {
        map.SetStartPoint(occupation.origin);
        GetComponentInChildren<SpriteRenderer>().enabled = false;
    }

}
