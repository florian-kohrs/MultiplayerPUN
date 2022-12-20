using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlammingPlattforms : BasePlaceableBehaviours
{

    public float timeToClose = 2f;

    public Transform left;
    public Transform right;
    public Transform center;

    public float Offset => left.localScale.x / 2;

    public float time = 0;

    private void Update()
    {
        time += Time.deltaTime;
        SetCompression();
    }

    public override void ResetOnNewRound(BaseMap map)
    {
        time = 0;
    }

    protected void SetCompression()
    {
        ///set distance between 0 and 1;
        float dist = Mathf.Cos(time / timeToClose * Mathf.PI) / 2 + 0.5f;

        left.localPosition = new Vector3(-dist - Offset, 0, 0);
        right.localPosition = new Vector3(dist + Offset, 0, 0);

        center.transform.localScale = new Vector3(0.25f, 2 * dist, 1);
    }

}
