using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlanetProjectile : MonoBehaviourPun
{

    protected Planet planet;

    protected Rigidbody body;

    protected Vector3 GravityDirection => transform.position - planet.transform.position;

    //protected override void OnStart()
    //{
    //    planet = Planet.PlanetRef;
    //}

    private void Start()
    {
        planet = Planet.PlanetRef;
        body = GetComponent<Rigidbody>();
    }


    protected void FixedUpdate()
    {
        transform.rotation = Quaternion.LookRotation(body.velocity, -GravityDirection);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
        PhotonView view = collision.transform.parent.GetComponentInChildren<PhotonView>();
        if (view != null && !view.IsMine)
            return;


        Rigidbody other = collision.rigidbody;
        if(other != null)
        {
            other.AddForce(body.velocity * (body.mass /  other.mass), ForceMode.Impulse);
            PlanetCharacterController controller = other.GetComponent<PlanetCharacterController>();
            if (controller != null)
            {
                controller.SendFlying(1);
            }
        }
    }

}
