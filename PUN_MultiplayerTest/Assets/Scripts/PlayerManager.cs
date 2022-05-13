using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : PunLocalBehaviour
{

    protected override void OnNotMine()
    {
        gameObject.tag = "OtherPlayers";
        base.OnNotMine();
    }

}
