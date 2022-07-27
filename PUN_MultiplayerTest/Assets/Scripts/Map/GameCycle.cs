using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCycle : MonoBehaviourPun
{

    public static GameCycle instance;

    public static Vector2Int MapSize => instance.map.Dimensions;

    public BaseMap map;

    public PlaceOnMap placeOnMap;

    public MapGenerator mapGenerator;

    public SelectItemToPlace selectItem;

    protected int selectedObjectIndex;

    protected List<Player> players;

    public GameObject startGameButton;

    public List<Action> OnGameStartCallback = new List<Action>();

    protected bool isInGame = false;

    private void Awake()
    {
        instance = this;
    }



    public static void AddGameStartCallback(Action onStart)
    {
        instance.OnGameStartCallback.Add(onStart);
    }

    public static bool GameStarted => instance.isInGame;

    public int MaxPlayers => players.Count;

    protected int remainingPlayersInRound;
    protected int remainingSelecting;
    protected int remainingPlacing;

    public static int NumberPlayers => Player.playerCount;

    protected int maxRounds;

    protected int currentRound;

    //TODO: Move Camera when placing 
    //Stop player after reaching target

    public void StartGame(int maxRounds, int seed)
    {
        startGameButton.SetActive(false);
        Broadcast.SafeRPC(photonView, nameof(StartGameBroadcast), RpcTarget.All, 
            delegate { StartGameBroadcast(maxRounds, seed,0); }, 
            maxRounds, seed);
    }


    protected int seed;

    public System.Random rand;

    [PunRPC]
    protected void StartGameBroadcast(int maxRounds, int seed, int mapIndex)
    {
        this.seed = seed;
        rand = new System.Random(seed);
        this.maxRounds = maxRounds;
        currentRound = 0;
        isInGame = true;
        OnGameStartCallback.ForEach(f => f());
        mapGenerator.StartMap(mapIndex);
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
        //players.ForEach(p => p.GetActualPlayer().gameObject.SetActive(false));
        remainingSelecting = MaxPlayers;
        selectItem.StartSelection(selected => { selectedObjectIndex = selected; CallPlayerDoneSelecting(); }, rand);
    }

    protected void EnterObjectPlacingPhase()
    {
        selectItem.DestroyAllButtons();
        //players.ForEach(p => p.GetActualPlayer().gameObject.SetActive(false));
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
