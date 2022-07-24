using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PunMasterBehaviour : MonoBehaviourPun
{

    protected void Start()
    {
        if (!PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected)
        {
            OnNotMaster();
        }
        else
        {
            OnStart();
        }
    }

    protected virtual void OnStart() { }

    protected virtual void OnNotMaster() { Destroy(this); }

}
