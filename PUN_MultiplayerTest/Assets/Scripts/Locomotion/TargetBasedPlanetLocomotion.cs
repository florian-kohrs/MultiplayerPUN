using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBasedPlanetLocomotion : AnimatedPlanetCharacterController
{

    public Vector3 targetPosition;

    public void SetTargetPosition(Vector3 v)
    {
        targetPosition = v;
    }

    private void Update()
    {
        if (targetPosition != default)
        {
            Vector3 shortestDistanceDirection = ShortestDistanceDirection(transform.position, targetPosition);
            Quaternion targetRotation = Quaternion.LookRotation(shortestDistanceDirection, transform.up);
            //transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime);
            transform.rotation = targetRotation;
            moveDir = new Vector2(0, 1);
        }
    }

    protected Vector3 ShortestDistanceDirection(Vector3 from, Vector3 to)
    {
        from = from.normalized;
        to = to.normalized;
        Vector3 greatCircleNormal = Vector3.Cross(to, from);
        return Vector3.Cross(from, greatCircleNormal).normalized;
    }

}
