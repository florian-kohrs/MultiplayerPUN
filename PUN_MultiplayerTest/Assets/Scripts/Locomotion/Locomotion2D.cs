using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locomotion2D : PunLocalBehaviour, IVector2InputListener
{
    
    [HideInInspector]
    public Rigidbody2D body;

    public Player player;

    public float fallMultiplier = 1.5f;

    public float lowJumpMultiplier = 2f;

    public float moveSpeed = 10;

    [Range(0,10)]
    public float jumpVelocity = 3f;

    protected bool IsFalling => body.velocity.y < 0;
    protected bool IsJumping => body.velocity.y > 0;

    protected bool IsIdle => MathF.Abs(body.velocity.y) < 0.0001f;

    protected float maxJumpTime = 0.5f;

    protected bool isHoldingJump;

    protected Vector2 inputDir = Vector2.zero;

    public bool CanMove => player.CanPlayerMove;

    // Start is called before the first frame update
    protected override void OnStart()
    {
        body = GetComponentInChildren<Rigidbody2D>();
        GameManager.InputHandler.input.PlayerMovement.Jump.performed += _ =>
        {
            JumpPressed();
        };        
        GameManager.InputHandler.input.PlayerMovement.Jump.canceled += _ => { isHoldingJump = false; };
        GameManager.InputHandler.AddMovementListener(this);
    }


    protected void JumpPressed()
    {
        if (isHoldingJump)
            return;
        isHoldingJump = true;
        //body.AddForce(new Vector2(0, jumpVelocity), ForceMode2D.Impulse); 
        body.velocity = new Vector2(body.velocity.x, jumpVelocity);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsFalling)
        {
            body.velocity += Time.deltaTime * Physics2D.gravity.y * fallMultiplier * Vector2.up;
        }
        else if (IsJumping && !isHoldingJump && CanMove)
        {
            body.velocity += Time.deltaTime * Physics2D.gravity.y * lowJumpMultiplier * Vector2.up;
        }

        if (CanMove)
            body.velocity = new Vector2(inputDir.x * moveSpeed, body.velocity.y);
        
    }

    public void OnVector2Input(Vector2 input)
    {
        inputDir = input;

        //if (input.y <= 0)
        //    isHoldingJump = false;

        //if (input.y < 0)
        //{
        //    if (IsIdle)
        //    {
        //        Duck();
        //    }
            
        //}
        //else
        //{
        //    if(isDucking)
        //        UnDuck();
        //    if (input.y > 0)
        //        JumpPressed();
        //}
    }

    private void UnDuck()
    {
        isDucking = false;
        Debug.Log("unducked");
    }
    protected bool isDucking;
    private void Duck()
    {
        isDucking = true;
        Debug.Log("Ducked");
    }

}
