using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Broadcast
{
    
    public static void SafeRPC(PhotonView view, string name, RpcTarget target, Action callIfNotConnected, params object[] parameters)
    {
        if(PhotonNetwork.IsConnected)
        {
            view.RPC(name, target, parameters);
        }
        else
        {
            callIfNotConnected?.Invoke();
        }
    }

}
