using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunMasterBehaviour :  Photon.Pun.MonoBehaviourPun
{

    protected void Start()
    {
        if (!PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected)
        {
            OnNotMine();
        }
        else
        {
            OnStart();
        }
    }

    protected virtual void OnStart() { }

    protected virtual void OnNotMine() { Destroy(this); }

}
