using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedPlanetCharacterController : PlanetCharacterController
{


    public AnimatorHandler animHandler;

    protected override void OnStart()
    {
        base.OnStart();
        animHandler.Initialize();
    }

    protected override void OnAfterUpdate()
    {
        animHandler.UpdateAnimatorValues(moveDir.x, moveDir.y);
    }

}
