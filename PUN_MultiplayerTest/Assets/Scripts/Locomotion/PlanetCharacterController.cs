using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlanetCharacterController : PunLocalBehaviour
{

    [SerializeField]
    protected static Transform planet;

    [Header("Movement Stats")]
    [SerializeField]
    protected float moveSpeed = 10;

    public float moveSpeedScale = 1;

    [SerializeField]
    protected float rotationSpeed;

    [SerializeField]
    protected float fallingSpeed = 45;

    [Header("Ground detection")]

    [SerializeField]
    protected float groundDetectionRayStartingPoint = 0.5f;

    [SerializeField]
    protected float minimumDistanceNeededToBeginFall = 1f;

    [SerializeField]
    protected float groundDirectionRayDistance = 0.2f;

    protected LayerMask ignoreForGroundCheck;

    [SerializeField]
    protected float inAirTime;

    public float InAirTime => inAirTime;

    protected bool IsInAir => inAirTime > 0;

    protected bool IsGrounded => !IsInAir;

    protected Rigidbody body;

    protected Vector2 moveDir;

    protected Vector3 MoveDir => new Vector3(moveDir.x, 0, moveDir.y);


    protected float colliderWidth = 0.5f;

    protected override void OnStart()
    {
        if(planet == null)
            planet = Planet.PlanetRef.transform;
        ignoreForGroundCheck = ~(1 << 2 | 1 << 8 | 1 << 11);
        body = GetComponent<Rigidbody>();
    }


    protected void FixedUpdate()
    {
        Vector3 moveDir = transform.TransformDirection(MoveDir);
        Vector3 down = (planet.transform.position - transform.position).normalized;
        ApplyRotationToPlanetGravity(transform);
        HandleMovement(moveDir);
        HandleFalling(moveDir, down);
        OnAfterUpdate();
    }

    protected virtual void OnAfterUpdate() { }

    protected void HandleMovement(Vector3 moveDir)
    {
        if (IsGrounded && !HasObstacleInFrontOfHim())
        {
            body.velocity = moveDir * moveSpeed * moveSpeedScale;
        }
    }

    protected bool HasObstacleInFrontOfHim()
    {
        return false;
    }

    protected void HandleFalling(Vector3 moveDir, Vector3 down)
    {
        Vector3 origin = transform.position;
        origin -= down * groundDetectionRayStartingPoint;

        //if(Physics.Raycast(origin, transform.forward, out hit, 0.4f))
        //{
        //    moveDir = Vector3.zero;
        //}

        if (IsInAir)
        {
            body.AddForce(down * fallingSpeed);
            //move body forward while falling
            body.AddForce(moveDir * fallingSpeed / 10f);
        }

       
        CheckForGround(origin, down);
    }

    protected void CheckForGround(Vector3 origin, Vector3 down)
    {
        RaycastHit hit;
        //origin = origin - down * (groundDirectionRayDistance);

        float nextFrameFallDistance = Vector3.Scale(down, body.velocity).magnitude * Time.deltaTime;
        nextFrameFallDistance = Mathf.Max(nextFrameFallDistance, minimumDistanceNeededToBeginFall) * transform.lossyScale.y;
        Debug.DrawRay(origin, down, Color.red, 0);
        if (Physics.Raycast(origin, down, out hit, nextFrameFallDistance, ignoreForGroundCheck))
        {
            //transform.position = Vector3.Lerp(transform.position, hit.point, fallingSpeed * Time.deltaTime);
            transform.position = hit.point;
            if (IsInAir)
            {
                body.velocity = Vector3.zero;
                inAirTime = 0;
            }
        }
        else
        {
            if (IsGrounded)
            {
                //body.velocity = body.velocity / 2;
            }
            inAirTime += Time.deltaTime;
        }
    }

    public void ApplyRotationToPlanetGravity(Transform t)
    {
        ApplyRotationToPlanetGravity(t, planet.position);
    }

    public static void ApplyRotationToPlanetGravity(Transform t, Vector3 center)
    {
        t.rotation = Quaternion.LookRotation(t.position - center, -t.forward);
        t.Rotate(new Vector3(90, 0, 0), Space.Self);
    }

    public static Quaternion GetRotationToPlanetGravity(Vector3 pos, Vector3 center)
    {
        Vector3 forward = Random.insideUnitSphere;
        Vector3 up = pos - center;
        forward = Vector3.Cross(up, forward);
        return Quaternion.LookRotation(forward, up);
    }

}
