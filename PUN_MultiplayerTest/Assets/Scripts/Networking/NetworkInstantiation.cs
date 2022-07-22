using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkInstantiation : MonoBehaviour
{
    public static GameObject Instantiate(GameObject instantiate, Vector3 position, Quaternion rotation)
    {
        GameObject result;
        if (PhotonNetwork.IsConnected)
            result = PhotonNetwork.Instantiate(instantiate.name, position, rotation);
        else
            result = Object.Instantiate(instantiate, position, rotation);
        return result;
    }

}
