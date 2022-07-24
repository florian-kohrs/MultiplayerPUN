using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCycle : MonoBehaviourPun
{

    public static GameCycle instance;

    public BaseMap map;

    public PlaceOnMap placeOnMap;

    public MapGenerator mapGenerator;

    public SelectItemToPlace selectItem;

    protected int selectedObjectIndex;

    protected List<Player> players;

    public GameObject startGameButton;

    private void Awake()
    {
        instance = this;
    }

    public int MaxPlayers => players.Count;

    protected int remainingPlayersInRound;
    protected int remainingSelecting;
    protected int remainingPlacing;

    protected int maxRounds;

    protected int currentRound;

    //TODO: Move Camera when placing 
    //Stop player after reaching target

    public void StartGame(int maxRounds, int seed)
    {
        startGameButton.SetActive(false);
        Broadcast.SafeRPC(photonView, nameof(StartGameBroadcast), RpcTarget.All, 
            delegate { StartGameBroadcast(maxRounds, seed); }, 
            maxRounds, seed);
    }


    [PunRPC]
    protected void StartGameBroadcast(int maxRounds, int seed)
    {
        this.maxRounds = maxRounds;
        currentRound = 0;
        mapGenerator.Generate();
        BeginRound();
    }


    public void PlayerDoneRunning()
    {
        remainingPlayersInRound--;
        if(remainingPlayersInRound <= 0)
        {
            currentRound++;
            if(currentRound >= maxRounds)
                EndGame();
            else
                EnterObjectSelectingPhase();
        }
    }

    protected void EndGame()
    {

    }

    protected void CallPlayerDoneSelecting()
    {
        Broadcast.SafeRPC(photonView, nameof(PlayerDoneSelecting), RpcTarget.All, PlayerDoneSelecting);
    }

    [PunRPC]
    public void PlayerDoneSelecting()
    {
        remainingSelecting--;
        if (remainingSelecting <= 0)
        {
            EnterObjectPlacingPhase();
        }
    }

    protected void CallPlayerDonePlacing()
    {
        Broadcast.SafeRPC(photonView, nameof(PlayerDonePlacing), RpcTarget.All, PlayerDonePlacing);
    }

    [PunRPC]
    public void PlayerDonePlacing()
    {
        remainingPlacing--;
        if (remainingPlacing <= 0)
        {
            BeginRound();
        }
    }

    protected void EnterObjectSelectingPhase()
    {
        players.ForEach(p => p.GetActualPlayer().gameObject.SetActive(false));
        remainingSelecting = MaxPlayers;
        selectItem.StartSelection(selected => { selectedObjectIndex = selected; CallPlayerDoneSelecting(); });
    }

    protected void EnterObjectPlacingPhase()
    {
        players.ForEach(p => p.GetActualPlayer().gameObject.SetActive(false));
        remainingPlacing = MaxPlayers;
        placeOnMap.BeginPlace(selectedObjectIndex, CallPlayerDonePlacing);
    }

    protected void BeginRound()
    {
        players = Player.players;
        remainingPlayersInRound = MaxPlayers;
        ResetPlayers();
    }

    protected void ResetPlayers()
    {
        map.PositionPlayers(players);
        foreach (Player player in players)
        {
            player.ResetValues();
            player.GetActualPlayer().gameObject.SetActive(true);
        }
    }


}
