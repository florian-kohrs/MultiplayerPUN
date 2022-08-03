using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PointAnimator : MonoBehaviour
{

    public const float POINTS_ANIMATION_TIME = 0.75f;

    public const float IDLE_AFTER_SINGLE_ANIM = 0.1f;

    public static readonly Color DEATH_COLOR = Color.gray;

    public static readonly Color FINISH_COLOR = Color.green;

    [SerializeField]
    protected GameObject pointImage;

    protected PlayerState respectivePlayer;

    [SerializeField]
    protected RectTransform pointsParent;

    [SerializeField]
    protected TextMeshProUGUI text;

    protected float accumulatedAnimationWidth;

    protected float MaxPointsWidth => pointsParent.rect.width;

    protected int maxPoints;

    protected int totalPoints;

    protected Action onDoneSingle;
    protected Action onDoneAllOfTask;

    public void Initialize(PlayerState playerState, int maxPoints)
    {
        respectivePlayer = playerState;
        text.text = playerState.playerName;
        this.maxPoints = maxPoints;
    }

    public bool HasWon => totalPoints > maxPoints;

    protected float PointsToWidth(int points)
    {
        return MaxPointsWidth * Mathf.InverseLerp(0, maxPoints, points);
    }

    protected void AnimatePoints(int points, Color color, Action onDoneSingle)
    {
        this.onDoneSingle = onDoneSingle;
        totalPoints += points;
        float width = PointsToWidth(points);
        AnimatePointImage(width, color);
        accumulatedAnimationWidth += width;
    }

    public void AnimateFinish(Action onDone)
    {
        int pointsForFinish = (int)(GameCycle.FINISH_POINTS * respectivePlayer.arrivedPointsInRound);
        if (pointsForFinish > 0)
        {
            AnimatePoints(pointsForFinish, FINISH_COLOR, onDone);
        }
        else
        {
            onDone();
        }
    }

    public void AnimateDeathPoints(Action onDone)
    {
        onDoneAllOfTask = onDone;
        AnimateSingleDeathPoint();
    }

    protected void AnimateSingleDeathPoint()
    {
        if(respectivePlayer.killedInRound <= 0)
        {
            onDoneAllOfTask();
            return;
        }    
        respectivePlayer.killedInRound--;
        AnimatePoints(GameCycle.KILL_POINTS, DEATH_COLOR, AnimateSingleDeathPoint);
    }



    protected void AnimatePointImage(float width, Color color)
    {
        GameObject image = Instantiate(pointImage);
        image.transform.SetParent(pointsParent,false);
        image.transform.localPosition = new Vector3(accumulatedAnimationWidth, 0,0);
        RectTransform rectTransform = image.GetComponent<RectTransform>();
        image.GetComponent<Image>().color = color;
        StartCoroutine(ImageGrower(rectTransform, width));
    }

    protected IEnumerator ImageGrower(RectTransform t, float targetWidth)
    {
        t.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        float time = 0;
        while(time < POINTS_ANIMATION_TIME)
        {
            time += Time.deltaTime;
            float progress = Mathf.InverseLerp(0,POINTS_ANIMATION_TIME, time);
            t.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, progress * targetWidth);
            yield return null;
        }
        t.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetWidth);
        yield return new WaitForSeconds(IDLE_AFTER_SINGLE_ANIM);
        onDoneSingle();
    }


    //public GameObject playerPointDisplay;

    //public UIRowsOnlyGrid pointsGrid;





}
