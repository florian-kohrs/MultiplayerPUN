using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrounded : PunLocalBehaviour
{

    public Locomotion2D locomotion;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        locomotion.EnterGrounded();
    }

}
