using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviourPun
{

    public PlayerState GetPlayerFromId(int id)
    {
        if (PhotonNetwork.IsConnected)
            return (PlayerState)PhotonNetwork.CurrentRoom.Players[id].TagObject;
        else
            return this;
    }

    public int OwnerActorNumber
    {
        get
        {
            if (PhotonNetwork.IsConnected)
                return photonView.OwnerActorNr;
            else
                return 0;
        }
    }

    public Rigidbody2D body;

    public CameraMover camMover;

    public string playerName;

    protected bool isAlive = true;

    public bool CanPlayerMove => IsAlive && !HasReachedTarget;

    public bool FreezePlayer => !IsAlive || HasReachedTarget;

    public int Points;

    protected int killedInRound = 0;

    protected float arrivedPointsInRound = 0;

    public int KilledInRound
    {
        get
        {
            return killedInRound;
        }
        set
        {
            killedInRound++;
        }
    }


    private void Start()
    {
        if(PhotonNetwork.IsConnected)
            PhotonNetwork.CurrentRoom.Players[photonView.OwnerActorNr].TagObject = this;

        transform.position = Vector3.zero;

        if(photonView.IsMine)
        {
            transform.GetChild(0).Translate(new Vector3(OwnerActorNumber, 0, 0));
        }
    }

    public Transform GetActualPlayer() => transform.GetChild(0);


    public bool KillPlayer(int killeyById)
    {
        if (!hasReachedTarget && isAlive)
        {
            int actorId = OwnerActorNumber;
            bool selfKill = killeyById == actorId;
            isAlive = false;
            SwitchCameras();
            Broadcast.SafeRPC(PhotonView.Get(this), nameof(PlayerDied), RpcTarget.All, ()=>PlayerDied(actorId, selfKill), actorId, selfKill);
        }
        return !isAlive;
    }

    protected void SwitchCameras()
    {
        if(camMover != null)
        {
            camMover.SetToSpectateView();
        }
    }

    [PunRPC]
    protected void PlayerDied(int killedByPlayerID, bool killerGetsPoints)
    {
        isAlive = false;
        if (killerGetsPoints)
        {
            GetPlayerFromId(killedByPlayerID).killedInRound++;
            Debug.Log(GetPlayerFromId(killedByPlayerID).playerName + " got points for a kill;");
        }
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

    public bool SetPlayerReachedTarget(int playerId)
    {
        if (isAlive && !hasReachedTarget)
        {
            SwitchCameras();
            hasReachedTarget = true;
            body.velocity = default;
            Broadcast.SafeRPC(PhotonView.Get(this), nameof(PlayerArrived), RpcTarget.All, delegate { PlayerArrived(playerId); },playerId);
            return true;
        }
        return false;
    }

    [PunRPC]
    protected void PlayerArrived(int playerId)
    {
        hasReachedTarget = true;
        body.velocity = default;
        GetPlayerFromId(playerId).arrivedPointsInRound += GameCycle.GetFinishReward();

        GameCycle.instance.PlayerDoneRunning();
    }

    public bool HasReachedTarget
    {
        get { return hasReachedTarget;}
    }


}
