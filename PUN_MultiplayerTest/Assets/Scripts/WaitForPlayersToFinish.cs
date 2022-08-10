using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForPlayersToFinish
{

    /// <summary>
    /// active players, where key is the player room number and value is done or not
    /// </summary>
    protected Dictionary<PlayerState,bool> activePlayers;

    protected Action onAllPlayersDone;

    public bool isActive;

    protected string taskName;

    public WaitForPlayersToFinish(string taskName, Action onAllPlayersDone)
    {
        this.taskName = taskName;
        this.onAllPlayersDone = onAllPlayersDone;
        NetworkGameManager.AddPlayerDisconnectedListener(OnPlayerDisconnect);
    }

    public void StartWaitingForPlayers()
    {
        isActive = true;
        activePlayers = new Dictionary<PlayerState, bool>();
        GameCycle.IterateOverPlayers(p => activePlayers.Add(p, false));
    }

    protected void OnPlayerDisconnect(Player p)
    {
        //if(isActive)
        //    PlayerFinished(p.TagObject as PlayerState);
        activePlayers.Remove(p.TagObject as PlayerState);
    }



    public bool AllDone => !activePlayers.ContainsValue(false);

    public bool AnyDone => activePlayers.ContainsValue(true);

    public void PlayerFinished(PlayerState player)
    {
        if (!isActive)
            throw new Exception($"Player {player.playerName} is done with an inactive task {taskName}");

        activePlayers[player] = true;
        if(AllDone)
        {
            isActive = false;
            onAllPlayersDone();
        }
    }

}
