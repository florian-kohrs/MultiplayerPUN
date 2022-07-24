using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkInstantiation : MonoBehaviour
{
    public static GameObject InstantiateOverNetwork(GameObject instantiate, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        GameObject result;

        if (PhotonNetwork.IsConnected)
            result = PhotonNetwork.Instantiate("MapObstacles/" + instantiate.name, position, rotation);
        else
            result = Instantiate(instantiate, position, rotation);

        result.transform.localScale = scale;
        return result;
    }

}
