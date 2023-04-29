using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerBeltBehaviour : BasePlaceableBehaviours
{

    public List<Rotate> rotators;

    public float rotateSpeed = -60;

    public float transportSpeed = 5;

    public BoxCollider2D conveyerTrigger;

    protected Transform localPlayer;
    protected BoxCollider2D feet;

    private void Start()
    {
        foreach (var item in rotators)
        {
            item.rotateSpeed = rotateSpeed;
        }
        localPlayer = PlayerState.GetLocalPlayerTransform();
        if(localPlayer != null)
            feet = localPlayer.parent.GetComponentInChildren<PlayerGrounded>().GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (feet == null)
            return;

        if (conveyerTrigger.IsTouching(feet))
        {
            localPlayer.position += new Vector3(Mathf.Sign(transform.localScale.x) * Time.deltaTime * transportSpeed, 0, 0);
        }
    }

}
