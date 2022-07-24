using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishCollision : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player p = collision.gameObject.GetComponentInParent<Player>();
        if (p != null && p.SetPlayerReachedTarget())
        {
            Debug.Log($"Player {p.playerName} reached target");
        }
    }


}
