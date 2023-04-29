using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerState : MonoBehaviourPun
{

    public static Transform GetLocalPlayerTransform()
    {
        return GetLocalPlayer()?.transform.GetChild(0);
    }

    public static bool HasPlayer => GetLocalPlayer() != null;

    public static PlayerState GetLocalPlayer()
    {
        if(PhotonNetwork.IsConnected)
            return PhotonNetwork.LocalPlayer.TagObject as PlayerState;
        else 
            return FindObjectOfType<PlayerState>();
    }

    public static Player GetPlayerFromId(int id)
    {
        if (PhotonNetwork.IsConnected)
            return PhotonNetwork.CurrentRoom.Players.Values.Where(p => p.ActorNumber == id).FirstOrDefault();
        else
            return null;
    }

    public static List<PlayerState> AllPlayers
    {
        get
        {
            if (PhotonNetwork.IsConnected)
                return PhotonNetwork.CurrentRoom.Players.Values.Select(p => p.TagObject as PlayerState).ToList();
            else
                return new List<PlayerState>() { GetLocalPlayer() };
        }
    }

    public PlayerState GetPlayerStateFromId(int id)
    {
        if (PhotonNetwork.IsConnected)
            return (PlayerState)GetPlayerFromId(id).TagObject;
        else
            return this;
    }

    public static PlayerState GetPlayerStaticStateFromId(int id)
    {
        if (PhotonNetwork.IsConnected)
            return (PlayerState)GetPlayerFromId(id).TagObject;
        else
            return FindObjectOfType<PlayerState>();
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
            GameCycle.instance.PlayerDoneRunning(this);
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
        GetActualPlayer().transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
        body.velocity = default;
        hasReachedTarget = false;
        isAlive = true;
    }

    public bool SetPlayerReachedTarget(int playerId)
    {
        if (isAlive && !hasReachedTarget)
        {
            hasReachedTarget = true;
            body.velocity = default;
            Debug.Log($"Player {GetPlayerStateFromId(playerId).playerName} has reached his target");
            SwitchCameras();
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

        GameCycle.instance.PlayerDoneRunning(this);
    }

    public bool HasReachedTarget
    {
        get { return hasReachedTarget;}
    }


}
