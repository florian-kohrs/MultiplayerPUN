using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlaceableBehaviours : MonoBehaviour
{

    public MapOccupation occupation;

    public virtual void OnPlace(BaseMap map) { }

}
