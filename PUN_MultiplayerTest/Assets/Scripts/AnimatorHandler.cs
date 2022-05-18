using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorHandler : PunLocalBehaviour
{

    public Animator anim;

    protected int vertical;
    protected int horizontal;

    protected bool canRotate = true;

    public bool IsInteracting => anim.GetBool("isInteracting");
    
    public bool CanRotate => canRotate;

    public void Initialize()
    {
        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");
    }

    public void UpdateAnimatorValues(float vertical, float horizontal)
    {
        float v = 0;
        float signV = Mathf.Sign(vertical);
        float absV = Mathf.Abs(vertical);
        if (absV > 0 && absV < 0.55f)
            v = signV * 0.5f;
        else if (absV > 0.55f)
            v = signV * 1;


        float h = 0;
        float signH = Mathf.Sign(horizontal);
        float absH = Mathf.Abs(horizontal);
        if (absH > 0 && absH < 0.55f)
            h = signH * 0.5f;
        else if (absH > 0.55f)
            h = signH * 1;

        anim.SetFloat(this.vertical, v, 0.1f, Time.deltaTime);
        anim.SetFloat(this.horizontal, h, 0.1f, Time.deltaTime);
    }



    public void PlayTargetAnimation(string targetAnimName, bool isInteracting, float transitionTime = 0.2f)
    {
        anim.SetBool("isInteracting", isInteracting);
        anim.CrossFade(targetAnimName, transitionTime);
    }

    public void ReturnToDefaultPosition()
    {
        PlayTargetAnimation("Default", false, 0.4f);
    }

    public void EnableRotate()
    {
        canRotate = true;
    }

    public void DisableRotate()
    {
        canRotate = false;
    }


}
