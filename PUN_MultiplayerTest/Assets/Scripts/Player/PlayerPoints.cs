using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerPoints : MonoBehaviour
{
    
    protected List<PointAnimator> animators;

    public UIRowsOnlyGrid pointsGrid;

    public GameObject playerPointUI;

    public float offset = 110;

    protected Action onAnimationDone;

    private void Start()
    {
        pointsGrid.gameObject.SetActive(false);
        GameCycle.AddGameStartCallback(OnGameStarted);
    }

    protected void OnGameStarted()
    {
        animators = new List<PointAnimator>();
        GameCycle.IterateOverPlayers(CreateUIForPlayer);
    }

    protected void CreateUIForPlayer(PlayerState player)
    {
        GameObject playerUI = Instantiate(playerPointUI);
        pointsGrid.AddChildWithIndex(playerUI.transform, player.OwnerActorNumber);
        PointAnimator animator = playerUI.GetComponent<PointAnimator>();
        animators.Add(animator);
        animator.Initialize(player, GameCycle.MaxPointsToFinish);
    }

    public void AnimatePointsForRound(Action onDoneAnimating)
    {
        pointsGrid.gameObject.SetActive(true);
        this.onAnimationDone = onDoneAnimating;
        this.DoDelayed(1, () =>
        {
            AnimateDeath();
        });
    }

    public bool HasPlayerWon()
    {
        return animators.Any(a => a.HasWon);
    }

    protected void DoForAllAndWaitForAll(Action<PointAnimator, Action> doStuff, Action onAllDone)
    {
        int count = animators.Count;
        int doneCount = 0;
        foreach (PointAnimator anim in animators)
        {
            doStuff(anim, () =>
            {
                doneCount++;
                if(doneCount >= count)
                    onAllDone();
            });
        }
    }

    protected void AnimateDeath()
    {
        DoForAllAndWaitForAll((anim, f) => anim.AnimateDeathPoints(f), AnimateFinishes);
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
