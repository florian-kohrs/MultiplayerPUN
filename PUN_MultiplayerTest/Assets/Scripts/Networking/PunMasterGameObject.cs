using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunMasterGameObject : PunMasterBehaviour
{

    protected override void OnNotMaster()
    {
        Destroy(gameObject);
    }

}
