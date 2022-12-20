using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : BasePlaceableBehaviours
{

    public override void OnPlace(BaseMap map)
    {
        map.DestroyOccupations(occupation, true);
        //Todo: Play particle effect
        Destroy(gameObject);
    }

}
