using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChicken : RangeAggressionTrigger
{

    public TargetBasedPlanetLocomotion locomotion;


    private void Update()
    {
        if(target != null)
        {
            locomotion.SetTargetPosition(target.transform.position);
        }
    }

    protected override void OnLooseTarget()
    {
        locomotion.SetTargetPosition(Vector3.zero);
    }

}
