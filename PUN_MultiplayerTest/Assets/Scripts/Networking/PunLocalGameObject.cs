using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunLocalGameObject : PunLocalBehaviour
{

    protected override void OnNotMine()
    {
        Destroy(gameObject);
    }

}
