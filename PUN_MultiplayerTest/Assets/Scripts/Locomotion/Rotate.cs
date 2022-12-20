using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : BasePlaceableBehaviours
{

    public float rotateSpeed;

    protected Quaternion startRotation;

    private void Start()
    {
        startRotation = transform.rotation;
    }

    void Update()
    {
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
    }

    public override void ResetOnNewRound(BaseMap map)
    {
        transform.rotation = startRotation;
    }

}
