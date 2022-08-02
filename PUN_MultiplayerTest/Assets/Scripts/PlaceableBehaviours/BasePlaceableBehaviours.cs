using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlaceableBehaviours : MonoBehaviour, IHasPlacedById
{

    [HideInInspector]
    public MapOccupation occupation;

    public int placedByPlayerId;

    public int PlacedByPlayerID { get => placedByPlayerId; set => placedByPlayerId = value; }

    public virtual void OnPlace(BaseMap map) { }

}
