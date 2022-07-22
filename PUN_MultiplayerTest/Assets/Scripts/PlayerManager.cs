using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : PunLocalBehaviour
{

    public Transform increaseLOD;
    public Transform decreaseLOD;

    protected override void OnNotMine()
    {
        gameObject.tag = "OtherPlayers";
        base.OnNotMine();
    }

}
