using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locomotion2D : PunLocalBehaviour, IVector2InputListener
{
    
    [HideInInspector]
    public Rigidbody2D body;

    protected Vector3 Origin => body.position;

    public PlayerState player;

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

    protected const float SLIDING_VELOCITY = 2;

    protected Vector2 inputDir = Vector2.zero;

    protected bool didSlidingJump = false;

    public bool CanMove => player.CanPlayerMove;

    protected IEnumerator slidingReset;

    protected bool canJump = true;

    public SideCollider rightSide;

    public SideCollider leftSide;

    protected bool IsSlidingRight => !didSlidingJump && rightSide.IsTouching;
    protected bool IsSlidingLeft => !didSlidingJump && leftSide.IsTouching;

    protected int SlidingJumpDirection
    {
        get
        {
            int sign = 0;
            if(IsSlidingRight && inputDir.x > 0)
                sign = 1;
            else if(IsSlidingLeft && inputDir.x < 0)
                sign = -1;
            return sign;
        }
    }

    protected bool CanDoSlidingJump => SlidingJumpDirection != 0;

    public void EnterGrounded()
    {
        canJump = true;
        didSlidingJump = false;
        if(slidingReset != null)
            StopCoroutine(slidingReset);
    }

    protected LayerMask mapCollisionLayer;

    // Start is called before the first frame update
    protected override void OnStart()
    {
        mapCollisionLayer = LayerMask.NameToLayer("MapOccupation");
        body = GetComponentInChildren<Rigidbody2D>();
        body.gravityScale = 0;
        GameCycle.AddGameStartCallback(() => body.gravityScale = 1);
        GameManager.InputHandler.input.PlayerMovement.Jump.performed += _ =>
        {
            JumpPressed();
        };        
        GameManager.InputHandler.input.PlayerMovement.Jump.canceled += _ => { isHoldingJump = false; };
        GameManager.InputHandler.AddMovementListener(this);
    }


    protected override void OnNotMine()
    {
        body = GetComponentInChildren<Rigidbody2D>();
        body.gravityScale = 0;
        base.OnNotMine();
    }

    protected void JumpPressed()
    {
        if (isHoldingJump)
            return;

        
        isHoldingJump = true;
        if (CanDoSlidingJump)
            ExecuteSlidingJump();
        else if(canJump)
            ExecuteNormalJump();

        canJump = false;
    }

    protected void ExecuteNormalJump()
    {
        body.velocity = new Vector2(body.velocity.x, jumpVelocity);
    }

    protected void ExecuteSlidingJump()
    {
        if (slidingReset != null)
        {
            StopCoroutine(slidingReset);
        }
        didSlidingJump = true;
        slidingReset = this.DoDelayed(0.15f, () => didSlidingJump = false);
        float jumpDegree = 45;
        float powerXAxis = Mathf.Sin(Mathf.Deg2Rad * jumpDegree);
        float powerYAxis = Mathf.Cos(Mathf.Deg2Rad * jumpDegree);
        body.velocity = new Vector2(powerXAxis * jumpVelocity * -1 * Mathf.Sign(inputDir.x), powerYAxis * jumpVelocity);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameCycle.GameStarted)
        {
            if (IsFalling)
            {
                if (CanDoSlidingJump)
                {
                    body.velocity = new Vector2(body.velocity.x, -SLIDING_VELOCITY);
                }
                else
                {
                    body.velocity += Time.deltaTime * Physics2D.gravity.y * fallMultiplier * Vector2.up;
                }
            }
            else if (IsJumping && !isHoldingJump && CanMove)
            {
                 body.velocity += Time.deltaTime * Physics2D.gravity.y * lowJumpMultiplier * Vector2.up;
            }
        }
        else 
        {
            body.velocity = new Vector2(body.velocity.x, 0);
        }

        ///lock movement for short time after wall jump
        if (CanMove && !didSlidingJump)
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

    //protected bool IsSlidingOnWall()
    //{
    //    float xDir = inputDir.x;
    //    bool result = xDir != 0;
    //    ///if no input on x axis is made player doesnt slide on wall
    //    if (!result || didSlidingJump)
    //        return false;

    //    xDir = MathF.Sign(xDir);
    //    ///if a wall jump was just made ignore wall for sliding player just jumped off
    //    if (xDir != Mathf.Sign(body.velocity.x))
    //        return false;


    //    RaycastHit2D hit = Physics2D.Raycast(Origin, new Vector2(xDir, 0), 0.55f, 1 << mapCollisionLayer);
    //    result = hit.collider != null;
    //    return result;
    //}

}
