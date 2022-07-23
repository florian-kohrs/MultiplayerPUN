using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCollision : PunLocalBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player p = collision.gameObject.GetComponentInChildren<Player>();
        if (p != null)
        {
            p.IsAlive = false;
            if (!p.IsAlive)
            {
                collision.rigidbody.AddForce(collision.relativeVelocity * 2, ForceMode2D.Impulse);
                Debug.Log($"Player {p.name} has died");
            }
        }
    }

}
