using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCycle : MonoBehaviourPun
{

    public const int KILL_POINTS = 75;

    public const int FINISH_POINTS = 200;

    public const float FINISH_FIRST_MULTIPLIER = 1.5f;

    public int maxPointsToFinish = 1200;

    public static int MaxPointsToFinish => instance.maxPointsToFinish;

    public static GameCycle instance;

    public static Vector2Int MapSize => instance.map.Dimensions;

    public BaseMap map;

    public PlaceOnMap placeOnMap;

    public MapGenerator mapGenerator;

    public SelectItemToPlace selectItem;

    protected int selectedObjectIndex;

    public GameObject startGameButton;

    public List<Action> OnGameStartCallback = new List<Action>();

    public PlayerPoints playerPoints;

    protected WaitForPlayersToFinish waitForPlayersRunning;
    protected WaitForPlayersToFinish waitForPlayersSelecting;
    protected WaitForPlayersToFinish waitForPlayersPlacing;




    protected bool isInGame = false;


    public static bool IsFirst => !instance.waitForPlayersRunning.AnyDone;

    public static float GetFinishReward()
    {
        if (IsFirst)
            return FINISH_FIRST_MULTIPLIER;
        else
            return 1;
    }

    private void Awake()
    {
        instance = this;
        waitForPlayersRunning = new WaitForPlayersToFinish("Running", PlayersDoneRunning);
        waitForPlayersSelecting = new WaitForPlayersToFinish("Selecting", EnterObjectPlacingPhase);
        waitForPlayersPlacing = new WaitForPlayersToFinish("Placing", BeginRound);
    }

    protected int ID
    {
        get
        {
            if (PhotonNetwork.IsConnected)
                return photonView.OwnerActorNr;
            else
                return 0;
        }
    }

    public void AnimatePoints()
    {
        playerPoints.AnimatePointsForRound(EnterObjectSelectingPhase);
    }


    public static void AddGameStartCallback(Action onStart)
    {
        instance.OnGameStartCallback.Add(onStart);
    }

    public static bool GameStarted => instance.isInGame;

    public int MaxPlayers => NumberPlayers;

    public static int NumberPlayers
    {
        get
        {
            if (PhotonNetwork.IsConnected)
                return PhotonNetwork.CurrentRoom.PlayerCount;
            else
                return 1;
        }
    }

    protected int maxRounds;

    protected int currentRound;

    //TODO: Move Camera when placing 
    //Stop player after reaching target

    public void StartGame(int maxRounds, int seed, int selectedMapIndex)
    {
        startGameButton.SetActive(false);
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        Broadcast.SafeRPC(photonView, nameof(StartGameBroadcast), RpcTarget.All, 
            delegate { StartGameBroadcast(maxRounds, seed, selectedMapIndex); }, 
            maxRounds, seed, selectedMapIndex);
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

    public void PlayerDoneRunning(PlayerState p)
    {
        waitForPlayersRunning.PlayerFinished(p);
    }

    protected void PlayersDoneRunning()
    {
        currentRound++;
        if (currentRound >= maxRounds)
            EndGame();
        else
            AnimatePoints();
    }

    protected void IsGameDone()
    {

    }

    protected void EndGame()
    {
        playerPoints.AnimatePointsForRound(WrapGameUp);
    }

    protected void WrapGameUp()
    {


        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
        }
    }

    protected void CallPlayerDoneSelecting()
    {
        Broadcast.SafeRPC(photonView, nameof(PlayerDoneSelecting), RpcTarget.All, ()=>PlayerDoneSelecting(ID), ID);
    }

    [PunRPC]
    public void PlayerDoneSelecting(int playerId)
    {
        waitForPlayersSelecting.PlayerFinished(PlayerState.GetPlayerStaticStateFromId(playerId));
    }

    protected void CallPlayerDonePlacing()
    {
        Broadcast.SafeRPC(photonView, nameof(PlayerDonePlacing), RpcTarget.All, ()=>PlayerDonePlacing(ID), ID);
    }

    [PunRPC]
    public void PlayerDonePlacing(int playerId)
    {
        waitForPlayersPlacing.PlayerFinished(PlayerState.GetPlayerStaticStateFromId(playerId));
    }

    protected void EnterObjectSelectingPhase()
    {
        //players.ForEach(p => p.GetActualPlayer().gameObject.SetActive(false));
        waitForPlayersSelecting.StartWaitingForPlayers();
        selectItem.StartSelection(selected => { selectedObjectIndex = selected; CallPlayerDoneSelecting(); }, rand);
    }

    protected void EnterObjectPlacingPhase()
    {
        map.ActivateMapMarker(true);
        selectItem.ClearUpOnAllSelected();
        //players.ForEach(p => p.GetActualPlayer().gameObject.SetActive(false));
        waitForPlayersPlacing.StartWaitingForPlayers();
        placeOnMap.BeginPlace(selectedObjectIndex, CallPlayerDonePlacing);
    }

    protected void BeginRound()
    {
        SimulatePlayerPhysics();
        map.ActivateMapMarker(false);
        waitForPlayersRunning.StartWaitingForPlayers();
        ResetPlayers();
        map.StartNewRound();
        placeOnMap.CamMover.SetToGameView();
    }

    protected void SimulatePlayerPhysics()
    {
        PlayerState.GetLocalPlayer().body.simulated = true;
    }


    public static void IterateOverPlayers(Action<PlayerState> f)
    {
        if (PhotonNetwork.IsConnected)
        {
            foreach (var kv in PhotonNetwork.CurrentRoom.Players)
            {
                PlayerState p = kv.Value.TagObject as PlayerState;
                if (p == null)
                {
                    Debug.LogWarning("Player is null! Propably shouldnt be!");
                }
                else
                {
                    f(p);
                }
            }
        }
        else
        {
            foreach (var p in FindObjectsOfType<PlayerState>())
            {
                if (p == null)
                {
                    Debug.LogWarning("Player is null! Propably shouldnt be!");
                }
                else
                {
                    f(p);
                }
            }
        }
    }

    protected void ResetPlayers()
    {
        map.PositionPlayers();
        IterateOverPlayers((p) =>
        {
            p.ResetValues();
            p.GetActualPlayer().gameObject.SetActive(true);
        });
    }


}
