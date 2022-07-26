using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviourPun
{

    public static List<Player> players = new List<Player>();

    public Rigidbody2D body;

    public string playerName;

    protected bool isAlive = true;

    public bool CanPlayerMove => IsAlive && !HasReachedTarget;

    public bool FreezePlayer => !IsAlive || HasReachedTarget;

    public static int playerCount;

    private void Awake()
    {
        playerCount++;
        players.Add(this);
    }

    ~Player(){ players.Remove(this); playerCount--; }
    
    private void Start()
    {
        transform.position = Vector3.zero;
        if(photonView.IsMine)
        {
            transform.GetChild(0).Translate(new Vector3(playerCount, 0, 0));
        }
    }

    public Transform GetActualPlayer() => transform.GetChild(0);


    public bool KillPlayer()
    {
        if (!hasReachedTarget && isAlive)
        {
            isAlive = false;
            SwitchCameras();
            Broadcast.SafeRPC(PhotonView.Get(this), nameof(PlayerDied), RpcTarget.All, PlayerDied);
        }
        return isAlive;
    }

    protected void SwitchCameras()
    {

    }

    [PunRPC]
    protected void PlayerDied()
    {
        isAlive = false;
        GameCycle.instance.PlayerDoneRunning();
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
            SwitchCameras();
            hasReachedTarget = true;
            body.velocity = default;
            Broadcast.SafeRPC(PhotonView.Get(this), nameof(PlayerArrived), RpcTarget.All, PlayerArrived);
            return true;
        }
        return false;
    }

    [PunRPC]
    protected void PlayerArrived()
    {
        hasReachedTarget = true;
        body.velocity = default;
        GameCycle.instance.PlayerDoneRunning();
    }

    public bool HasReachedTarget
    {
        get { return hasReachedTarget;}
    }


}
