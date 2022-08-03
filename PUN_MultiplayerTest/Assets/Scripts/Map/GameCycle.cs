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

    protected bool isInGame = false;

    public static bool IsFirst => instance.remainingPlayersInRound == instance.MaxPlayers;

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

    protected int remainingPlayersInRound;
    protected int remainingSelecting;
    protected int remainingPlacing;

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



    public void PlayerDoneRunning()
    {
        remainingPlayersInRound--;
        if(remainingPlayersInRound <= 0)
        {
            currentRound++;
            if (currentRound >= maxRounds)
                EndGame();
            else
                AnimatePoints();
        }
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
        remainingPlayersInRound = MaxPlayers;
        ResetPlayers();
        map.StartNewRound();
        placeOnMap.CamMover.SetToGameView();
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
