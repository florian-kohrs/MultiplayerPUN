using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRigidBody : MonoBehaviour
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

    protected virtual void FixedUpdate()
    {
        Vector3 gravDir = GravityDirection;
        body.velocity += gravDir * planet.Gravity * Time.deltaTime;
    }

}
