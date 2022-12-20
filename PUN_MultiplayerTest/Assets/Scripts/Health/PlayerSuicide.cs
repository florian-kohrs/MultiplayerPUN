using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSuicide : MonoBehaviour
{

    public PlayerState state;

    protected float holdTime;

    public float timeBeforeDeath = 2;

    void Update()
    {
        if (Keyboard.current.tKey.isPressed)
            holdTime += Time.deltaTime;
        else
            holdTime = 0;

        if (holdTime > timeBeforeDeath)
            state.KillPlayer(-1);
    }
}
