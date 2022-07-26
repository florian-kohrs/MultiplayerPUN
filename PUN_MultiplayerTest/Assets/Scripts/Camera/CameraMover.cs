using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : PunLocalBehaviour
{

    public Camera mainCamera;
    public Camera MainCamera
    {
        get
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
            return mainCamera;
        }
    }

    public int defaultDistance = 6;
    public int placingDistance = 10;

    public Vector3 placingPosition;

    public Vector3 gamePosition;

    public Transform gameParent;

    protected bool isInGameView = true;

    public void SetToGameView()
    {
        if (isInGameView)
            return;

        isInGameView = true;
        MainCamera.transform.SetParent(gameParent);
        MainCamera.transform.localPosition = gamePosition;
        MainCamera.orthographicSize = defaultDistance;
    }

    public void SetToSpectateView()
    {
        if (!isInGameView)
            return;

        isInGameView = false;
        MainCamera.transform.SetParent(null);
        MainCamera.transform.position = placingPosition;
        MainCamera.orthographicSize = placingDistance;
    }

}
