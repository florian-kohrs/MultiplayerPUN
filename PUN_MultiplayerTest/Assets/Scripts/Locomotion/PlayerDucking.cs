using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDucking : PunLocalBehaviour
{

    //protected float 
    protected Vector3 duckScale = new Vector3(1.2f, 0.25f, 1);

    protected Transform LocalPlayer => PlayerState.GetLocalPlayerTransform();

    protected override void OnStart()
    {
        GameCycle.AddOnRoundEndCallback(RevertScale);
    }

    protected void RevertScale()
    {
        LocalPlayer.localScale = Vector3.one;
    }

    private void Update()
    {
        if (!GameCycle.AreRunning)
            return;

        if (Keyboard.current.sKey.isPressed)
            LocalPlayer.localScale = duckScale;
        else
            LocalPlayer.localScale = Vector3.one;
    }

}
