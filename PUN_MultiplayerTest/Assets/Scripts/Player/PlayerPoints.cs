using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerPoints : MonoBehaviour
{
    
    protected List<PointManager> animators;

    public UIRowsOnlyGrid pointsGrid;

    public GameObject playerPointUI;

    public float offset = 110;

    protected Action onAnimationDone;

    protected const string ALL_ALIVE_TEXT = "No deaths? No Points!";

    protected const string ALL_DEAD_TEXT = "No one of you imbeciles deserves any points!";

    protected const float DISPLAY_DURATION = 3;

    private void Start()
    {
        pointsGrid.gameObject.SetActive(false);
        GameCycle.AddGameStartCallback(OnGameStarted);
    }

    protected void OnGameStarted()
    {
        animators = new List<PointManager>();
        GameCycle.IterateOverPlayers(CreateUIForPlayer);
    }

    protected void CreateUIForPlayer(PlayerState player)
    {
        GameObject playerUI = Instantiate(playerPointUI);
        pointsGrid.AddChildWithIndex(playerUI.transform, player.OwnerActorNumber);
        PointManager animator = playerUI.GetComponent<PointManager>();
        animators.Add(animator);
        animator.Initialize(player, GameCycle.MaxPointsToFinish);
    }

    public void AnimatePointsForRound(Action onDoneAnimating)
    {
        pointsGrid.gameObject.SetActive(true);
        this.onAnimationDone = onDoneAnimating;

        if (PlayerState.AllPlayers.TrueForAll(p => p.IsAlive))
        {
            InfoTextDisplay.DisplayTextFor(ALL_ALIVE_TEXT, DISPLAY_DURATION);
            this.DoDelayed(DISPLAY_DURATION, () =>
            {
                EndAnimation();
            });
        }
        else if (PlayerState.AllPlayers.TrueForAll(p => !p.IsAlive))
        {
            InfoTextDisplay.DisplayTextFor(ALL_DEAD_TEXT, DISPLAY_DURATION);
            this.DoDelayed(DISPLAY_DURATION, () =>
            {
                EndAnimation();
            });
        }
        else
        {
            this.DoDelayed(1, () =>
            {
                AnimateKillPoints();
            });
        }
    }

    public bool HasPlayerWon()
    {
        return animators.Any(a => a.HasWon);
    }

    protected void DoForAllAndWaitForAll(Action<PointManager, Action> doStuff, Action onAllDone)
    {
        int count = animators.Count;
        int doneCount = 0;
        foreach (PointManager anim in animators)
        {
            doStuff(anim, () =>
            {
                doneCount++;
                if(doneCount >= count)
                    onAllDone();
            });
        }
    }

    protected void AnimateKillPoints()
    {
        DoForAllAndWaitForAll((anim, f) => anim.SetAndAnimateDeathPoints(f), AnimateFinishes);
    }

    protected void AnimateFinishes()
    {
        DoForAllAndWaitForAll((anim, f) => anim.AnimateFinish(f), ()=>this.DoDelayed(2, EndAnimation));
    }

    protected void EndAnimation()
    {
        pointsGrid.gameObject.SetActive(false); 
        onAnimationDone();
    }

    public List<int> GetPlayersWithMostPoints()
    {
        int maxPoints = animators.Max(a => a.TotalPoints);
        return animators.Where(a => a.TotalPoints >= maxPoints).Select(a => a.RespectivePlayer.OwnerActorNumber).ToList();
    }
}
