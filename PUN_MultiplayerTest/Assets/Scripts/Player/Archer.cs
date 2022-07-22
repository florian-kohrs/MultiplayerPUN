using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : MonoBehaviour
{

    public InputHandler inputHandler;

    public AnimatorHandler animator;

    protected bool isAiming;

    protected float aimTime = 0;

    protected const string AIMING_ANIMATION_PARAMATER_NAME = "IsAiming";

    void Update()
    {
        if (!isAiming && inputHandler.isAttacking)
            BeginDraw();
        else if (isAiming && !inputHandler.isAttacking)
            EndDraw();

        if (isAiming)
            aimTime += Time.deltaTime;
    }

    protected void BeginDraw()
    {
        isAiming = true;
        animator.anim.SetBool(AIMING_ANIMATION_PARAMATER_NAME, true);
    }

    protected void EndDraw()
    {
        isAiming = false;
        animator.anim.SetBool(AIMING_ANIMATION_PARAMATER_NAME, false);
    }

}
