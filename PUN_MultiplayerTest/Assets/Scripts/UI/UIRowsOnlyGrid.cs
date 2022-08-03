using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRowsOnlyGrid : MonoBehaviour
{

    public float spacing = 10;

    public float height = 100;

    protected float GetYOffset(int index) => (height + spacing) * index;
    
    public float startX = 0;
    public float startY = 0;

    protected Vector3 GetCurrentPosition(int index)
    {
        if (PhotonNetwork.IsConnected)
            index--;
        return new Vector3(startX, GetYOffset(index), 0);
    }

    public void AddChildWithIndex(Transform t, int index)
    {
        t.SetParent(transform, false);
        RectTransform rect = t.GetComponent<RectTransform>();
        rect.transform.localPosition -= GetCurrentPosition(index);
    }


}
