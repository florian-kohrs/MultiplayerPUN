using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishCollision : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerState p = collision.gameObject.GetComponentInParent<PlayerState>();
        if (p != null && p.SetPlayerReachedTarget(p.OwnerActorNumber))
        {
            Debug.Log($"Player {p.playerName} reached target");
        }
    }


}
