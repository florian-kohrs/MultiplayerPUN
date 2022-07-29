using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartItem : BasePlaceableBehaviours
{

    public override void OnPlace(BaseMap map)
    {
        map.SetStartPoint(occupation.origin);
        if(PhotonNetwork.IsConnected)
            GetComponentInChildren<SpriteRenderer>().enabled = false;
    }

}
