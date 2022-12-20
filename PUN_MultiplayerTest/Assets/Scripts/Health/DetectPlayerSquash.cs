using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayerSquash : MonoBehaviourPun
{

    private Rigidbody2D rigidbody2D;

    public float forceThreshold = 10f; // The threshold force required to be considered squashed
    public float raycastDistance = 0.05f; // The distance to raycast to check for a collider on the other side of the collision

    public PlayerState playerState;

    public LayerMask squashLayers;

    public float Size => transform.lossyScale.x - 0.3f;

    private void Start()
    {
        // Get the Rigidbody2D component of the object
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (PhotonNetwork.IsConnected && !photonView.IsMine)
            return;

        ///Death collision will kill player, no need to do any calculations here
        if (collision.transform.GetComponentInChildren<DeathCollision>() != null)
            return;


        float collisionImpulse = collision.relativeVelocity.magnitude / Time.fixedDeltaTime;

        // Check if the collision impulse is over the threshold
        //if (collisionImpulse < forceThreshold)
        //    return;
        

        // Calculate the size of the object in the direction of the collision normal
        //float size = Vector2.Dot(rigidbody2D.transform.lossyScale, collision.contacts[0].normal);

        // Cast a ray in the opposite direction of the collision normal to check for a collider on the other side
        RaycastHit2D hit = Physics2D.Raycast(collision.contacts[0].point + collision.contacts[0].normal * Size, collision.contacts[0].normal, raycastDistance, squashLayers);
        if (hit.collider != null)
        {
            var placeBy = collision.transform.GetComponentInChildren<IHasPlacedById>();
            int killedById = placeBy != null ? placeBy.PlacedByPlayerID : -1;
            playerState.KillPlayer(killedById);
            Debug.Log("Rigidbody is squashed!");
        }
    }

}
