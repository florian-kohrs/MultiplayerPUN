using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExceptionForwarder : MonoBehaviourPun
{

    private void Start()
    {
        Application.logMessageReceived += HandleException;
    }

    private void HandleException(string exception, string stackTrace, LogType type)
    {
        if (type == LogType.Exception)
        {
            string msg = exception + ": " + stackTrace;
            Broadcast.SafeRPC(photonView, nameof(PrintIfUnityEngine), RpcTarget.All, () => PrintIfUnityEngine(msg),msg);
        }
    }

    [PunRPC]
    public void PrintIfUnityEngine(string exceptionMessage)
    {
#if UNITY_EDITOR
        Debug.LogException(new System.Exception(exceptionMessage));
#endif
    }

}
