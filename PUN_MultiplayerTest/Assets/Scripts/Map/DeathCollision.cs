using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCollision : MonoBehaviour
{

    public float impactForce = 5;

    public bool breakOnCollision;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player p = collision.gameObject.GetComponentInParent<Player>();
        if (p != null && (!PhotonNetwork.IsConnected || p.photonView.IsMine) && !p.KillPlayer())
        { 
            collision.rigidbody.AddForce(collision.relativeVelocity * impactForce, ForceMode2D.Impulse);
            Debug.Log($"Player {p.playerName} has died");
        }
        if (breakOnCollision)
            Destroy(gameObject);
    }

}
