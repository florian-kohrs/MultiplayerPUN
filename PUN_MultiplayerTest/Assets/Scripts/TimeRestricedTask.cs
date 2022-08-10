using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeRestricedTask : MonoBehaviour
{

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI taskText;

    public GameObject timerUIParent;


    protected float maxTime;
    protected float timeLeft;
    protected string onTaskDoneText;

    protected Action onTimeRanOut;


    public void StartTask(float maxTime, Action timeRanOut, string taskDescription = "", string onTaskDoneText = "")
    {
        this.maxTime = maxTime;
        this.onTaskDoneText = onTaskDoneText;
        timeLeft = maxTime;
        onTimeRanOut = timeRanOut;
        taskText.text = taskDescription;
        enabled = true;
        timerUIParent.SetActive(true);
    }

    public void TaskDone()
    {
        taskText.text = onTaskDoneText;
        enabled = false;
        timerUIParent.SetActive(false);
    }


    void Start()
    {
        enabled = false;
        timerUIParent.SetActive(false);
    }

    void Update()
    {
        timeLeft -= Time.deltaTime;
        timerText.text = Mathf.CeilToInt(timeLeft).ToString();

        if (timeLeft > 0)
            return;

        TaskDone();
        onTimeRanOut();
    }
}
