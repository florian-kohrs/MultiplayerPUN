using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForPlayersToFinish : MonoBehaviour
{

    /// <summary>
    /// active players, where key is the player room number and value is done or not
    /// </summary>
    protected Dictionary<PlayerState,bool> activePlayers;

    protected Action onAllPlayersDone;

    public void StartWaitingForPlayers(Action onAllPlayersDone)
    {
        activePlayers = new Dictionary<PlayerState, bool>();
        GameCycle.IterateOverPlayers(p => activePlayers.Add(p, false));
    }

    protected void OnPlayerDisconnect(Player p)
    {
        p.tag
    }

    protected bool AllDone => !activePlayers.ContainsValue(false);

    private void Start()
    {
        
    }

    public void PlayerFinished(int playerId)
    {
        activePlayers
    }

}
