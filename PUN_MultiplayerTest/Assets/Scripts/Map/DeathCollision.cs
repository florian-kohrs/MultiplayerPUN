using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCollision : MonoBehaviour, IHasPlacedById
{

    public int placedByPlayerId;

    public float impactForce = 5;

    public bool breakOnCollision;

    public int PlacedByPlayerID
    {
        get
        {
            return placedByPlayerId;
        }
        set
        {
            placedByPlayerId = value;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerState p = collision.gameObject.GetComponentInParent<PlayerState>();
        if (p != null && (!PhotonNetwork.IsConnected || p.photonView.IsMine) && p.KillPlayer(placedByPlayerId))
        { 
            collision.rigidbody.AddForce(-collision.relativeVelocity * impactForce, ForceMode2D.Impulse);
            
            Debug.Log($"Player {p.playerName} has died");
        }
        if (breakOnCollision && collision.gameObject.GetComponent<Trampoline>() == null)
            Destroy(gameObject);
    }

}
