using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : PunLocalBehaviour
{

    protected PlayerInputActions input;

    [SerializeField]
    protected CameraController cameraHandler;


    public bool isAttacking;

    public bool isInteracting;

    private void OnEnable()
    {
        if(input == null)
        {
            input = new PlayerInputActions();
            input.PlayerMovement.Movement.performed += input => CallMoveListeners(input.ReadValue<Vector2>());
            input.PlayerMovement.Movement.canceled += _ => CallMoveListeners(Vector2.zero);
            input.PlayerMovement.Camera.performed += input => CallMouseListeners(input.ReadValue<Vector2>());
            input.PlayerMovement.Camera.canceled += _ => CallMouseListeners(Vector2.zero);
            input.PlayerActions.DefaultAttack.performed += _ => isAttacking = true;
            input.PlayerActions.DefaultAttack.canceled += _ => isAttacking = false;
        }
        input.Enable();
    }


    public void AddMovementListener(IVector2InputListener listener)
    {
        movementListeners.Add(listener);
    }


    public void RemoveMovementListener(IVector2InputListener listener)
    {
        movementListeners.Remove(listener);
    }

    public void AddMouseListener(IVector2InputListener listener)
    {
        mouseListeners.Add(listener);
    }


    public void RemoveMouseListener(IVector2InputListener listener)
    {
        mouseListeners.Remove(listener);
    }



    protected HashSet<IVector2InputListener> movementListeners = new HashSet<IVector2InputListener>();

    protected HashSet<IVector2InputListener> mouseListeners = new HashSet<IVector2InputListener>();

    protected void CallMoveListeners(Vector2 value)
    {
        foreach(IVector2InputListener listener in movementListeners)
            listener.OnVector2Input(value);
    }

    protected void CallMouseListeners(Vector2 value)
    {
        foreach (IVector2InputListener listener in mouseListeners)
            listener.OnVector2Input(value);
    }

    private void OnDisable()
    {
        input?.Disable();
    }

}
