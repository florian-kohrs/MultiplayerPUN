using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlanetCharakterController : AnimatedPlanetCharacterController, IVector2InputListener
{

    protected override void OnNotMine()
    {
        Collider[] c = GetComponentsInChildren<Collider>();
        for (int i = 0; i < c.Length; i++)
            Destroy(c[i]);
        base.OnNotMine();
    }

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
