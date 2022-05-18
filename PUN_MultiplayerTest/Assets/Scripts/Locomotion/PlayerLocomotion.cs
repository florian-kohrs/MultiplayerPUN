using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PlayerLocomotion : PunLocalBehaviour, IVector2InputListener
{

    protected InputHandler input;

    public Rigidbody body;

    public float moveSpeed = 5;

    public float rotationSpeed = 10;

    public AnimatorHandler animHandler;

    protected float moveScale = 1;

    public void SetMoveScale(float scale)
    {
        moveScale = scale;
    }

    protected override void OnStart()
    {
        body = GetComponent<Rigidbody>();
        input = GameManager.InputHandler;
        input.AddMovementListener(this);
        //animHandler = GetComponentInChildren<AnimatorHandler>();
        animHandler.Initialize();
    }


    protected Vector2 moveDir;

    private void Update()
    {
        Vector3 moveDirection = transform.TransformDirection(new Vector3(moveDir.x, 0, moveDir.y));
        moveDirection.Normalize();
        //Vector3 projectedVel = Vector3.ProjectOnPlane(moveDirection, Vector3.up);
        body.velocity = moveDirection * moveSpeed * moveScale;

        animHandler.UpdateAnimatorValues(moveDir.x, moveDir.y);
    }


    //protected override void OnNotMine()
    //{
    //    body = GetComponentInChildren<Rigidbody>();
    //    body.isKinematic = true;
    //    base.OnNotMine();
    //}

    public void OnVector2Input(Vector2 moveDir)
    {
        this.moveDir = moveDir;
    }

}
