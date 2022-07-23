using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public static List<Player> players = new List<Player>();

    public Rigidbody2D body;

    public string playerName;

    protected bool isAlive = true;

    public bool CanPlayerMove => IsAlive && !HasReachedTarget;

    public bool FreezePlayer => !IsAlive || HasReachedTarget;

    private void Awake()
    {
        players.Add(this);
    }

    public bool KillPlayer()
    {
        if (!hasReachedTarget && isAlive)
        {
            isAlive = false;
            GameCycle.instance.PlayerDoneRunning(this);
            Broadcast(/*Died*/);
        }
        return isAlive;
    }

    public void RevivePlayer()
    {
        isAlive = true;
    }

    public bool IsAlive
    {
        get { return isAlive;}
    }

    protected bool hasReachedTarget;

    public void ResetValues()
    {
        body.velocity = default;
        hasReachedTarget = false;
        isAlive = true;
    }

    public bool SetPlayerReachedTarget()
    {
        if (isAlive && !hasReachedTarget)
        {
            hasReachedTarget = true;
            body.velocity = default;
            GameCycle.instance.PlayerDoneRunning(this);
            Broadcast(/*Arrived?*/);
            return true;
        }
        return false;
    }

    public void ResetReachedTarget()
    {
        hasReachedTarget = false;
    }

    public bool HasReachedTarget
    {
        get { return hasReachedTarget;}
    }

    protected void Broadcast()
    {

    }

}
