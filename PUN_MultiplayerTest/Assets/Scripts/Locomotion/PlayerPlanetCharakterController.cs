using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlanetCharakterController : AnimatedPlanetCharacterController, IVector2InputListener
{

    public void OnVector2Input(Vector2 moveDir)
    {
        this.moveDir = moveDir;
    }

    protected override void OnStart()
    {
        base.OnStart();
        GameManager.InputHandler.AddMovementListener(this);
    }

}
