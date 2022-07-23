using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCycle : PunMasterBehaviour
{

    public static GameCycle instance;

    public BaseMap map;

    public PlaceOnMap placeOnMap;

    public MapGenerator mapGenerator;

    public SelectItemToPlace selectItem;

    protected MapOccupationObject selectedObject;

    protected List<Player> players;

    private void Awake()
    {
        instance = this;
    }

    public int MaxPlayers => players.Count;

    protected int remainingPlayersInRound;
    protected int remainingSelecting;
    protected int remainingPlacing;

    protected override void OnStart()
    {
        mapGenerator.Generate();
        BeginRound();
    }

    public void PlayerDoneRunning(Player p)
    {
        remainingPlayersInRound--;
        if(remainingPlayersInRound <= 0)
        {
            EnterObjectSelectingPhase();
        }
    }

    public void PlayerDoneSelecting()
    {
        remainingSelecting--;
        if (remainingSelecting <= 0)
        {
            EnterObjectPlacingPhase();
        }
    }

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
        players.ForEach(p => p.gameObject.SetActive(false));
        remainingSelecting = MaxPlayers;
        selectItem.StartSelection(selected => { selectedObject = selected; PlayerDoneSelecting(); });
    }

    protected void EnterObjectPlacingPhase()
    {
        players.ForEach(p => p.gameObject.SetActive(false));
        remainingPlacing = MaxPlayers;
        placeOnMap.BeginPlace(selectedObject, PlayerDonePlacing);
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
            player.gameObject.SetActive(true);
        }
    }


}
