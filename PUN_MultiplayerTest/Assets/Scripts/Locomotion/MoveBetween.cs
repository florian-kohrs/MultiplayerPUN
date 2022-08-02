using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBetween : BasePlaceableBehaviours
{
    public Transform from;
    public Transform to;

    public Transform moveThis;

    public float speed = 5;

    protected float neededTime;

    protected float time;

    private void Start()
    {
        neededTime = (from.position - to.position).magnitude / speed;
        moveThis.position = from.position;
    }

    private void Update()
    {
        time += Time.deltaTime;
        if(time > neededTime)
        {
            time -= neededTime;
            Transform temp = to;
            to = from;
            from = temp;
        }
        float progress = time / neededTime;
        moveThis.position = Vector3.Lerp(from.position, to.position, progress);
    }

}
