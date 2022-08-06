using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviourPun
{

    public static Transform GetLocalPlayerTransform()
    {
        return GetLocalPlayer().transform.GetChild(0);
    }

    public static bool HasPlayer => GetLocalPlayer() != null;

    public static PlayerState GetLocalPlayer()
    {
        if(PhotonNetwork.IsConnected)
            return PhotonNetwork.LocalPlayer.TagObject as PlayerState;
        else 
            return FindObjectOfType<PlayerState>();
    }

    public Player GetPlayerFromId(int id)
    {
        if (PhotonNetwork.IsConnected)
            return PhotonNetwork.CurrentRoom.Players[id];
        else
            return null;
    }

    public PlayerState GetPlayerStateFromId(int id)
    {
        if (PhotonNetwork.IsConnected)
            return (PlayerState)GetPlayerFromId(id).TagObject;
        else
            return this;
    }

    public Player GetPlayer(MonoBehaviourPun pun)
    {
        return GetPlayerFromId(pun.photonView.OwnerActorNr);
    }

    public PlayerState GetPlayerState(MonoBehaviourPun pun)
    {
        return (PlayerState)GetPlayer(pun).TagObject;
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
        


    public float pointMultiplier = 1;

    public Rigidbody2D body;

    public CameraMover camMover;

    public string playerName;

    protected bool isAlive = true;

    public bool CanPlayerMove => IsAlive && !HasReachedTarget;

    public bool FreezePlayer => !IsAlive || HasReachedTarget;

    public int killedInRound = 0;

    public float arrivedPointsInRound = 0;

    public void ResetRoundPoints()
    {
        killedInRound = 0;
        arrivedPointsInRound = 0;
    }

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
        if (PhotonNetwork.IsConnected)
        {
            Player p = GetPlayer(this);
            p.TagObject = this;
            playerName = p.NickName;
        }

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
            bool pointsForKill = killeyById > 0 && killeyById != actorId;
            isAlive = false;
            SwitchCameras();
            Broadcast.SafeRPC(PhotonView.Get(this), nameof(PlayerDied), RpcTarget.All, ()=>PlayerDied(killeyById, pointsForKill), killeyById, pointsForKill);
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
    protected void PlayerDied(int killedByPlayerID, bool pointsForKill)
    {
        isAlive = false;
        if (pointsForKill && killedByPlayerID > 0)
        {
            GetPlayerStateFromId(killedByPlayerID).killedInRound++;
            //Debug.Log(GetPlayerFromId(killedByPlayerID).playerName + " got points for a kill");
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
        ResetRoundPoints();
        GetLocalPlayerTransform().localScale = new Vector3(0.95f, 0.95f, 0.95f);
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
        GetPlayerStateFromId(playerId).arrivedPointsInRound += GameCycle.GetFinishReward();

        GameCycle.instance.PlayerDoneRunning();
    }

    public bool HasReachedTarget
    {
        get { return hasReachedTarget;}
    }


}
