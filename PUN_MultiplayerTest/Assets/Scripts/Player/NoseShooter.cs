using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoseShooter : PunLocalBehaviour
{

    public GameObject nosePrefab;

    public Transform activeNose;

    protected float noseGrowTime = 0.2f;

    public Vector3 defaultNoseScale;

    protected float attackAnimationInterval = 0.5f;

    public Vector3 attackMaxScale;
    public Vector3 attackMinScale;


    protected float attackTime;

    protected override void OnStart()
    {
        GameManager.InputHandler.RegisterEvent(i => i.PlayerActions.DefaultAttack.performed += delegate { Debug.Log("Start"); enabled = true;  }); 
        GameManager.InputHandler.RegisterEvent(i => i.PlayerActions.DefaultAttack.canceled += delegate { Debug.Log("Stop"); enabled = false; Attack(); });
        enabled = false;
        StartCoroutine(GrowNose());
    }

  
    protected IEnumerator GrowNose()
    {
        float growTime = 0;
        while (growTime < noseGrowTime)
        {
            growTime += Time.deltaTime;
            float growProgress = Mathf.Clamp01(growTime / noseGrowTime);
            ScaleOnFixedZAxis(activeNose, Vector3.Lerp(Vector3.zero, defaultNoseScale, growProgress));
            yield return null;
        }
    }

    protected void ScaleOnFixedZAxis(Transform t, Vector3 newScale)
    {
        float zDiff = newScale.z - t.localScale.z;
        t.transform.localPosition += new Vector3(0, 0, zDiff / 2);
        t.transform.localScale = newScale;
    }

    protected void Attack()
    {
        attackTime = 0;
        StartCoroutine(GrowNose());
    }

    void Update()
    {
        attackTime += Time.deltaTime;
        if (attackTime < attackAnimationInterval)
            ScaleOnFixedZAxis(activeNose, Vector3.Lerp(defaultNoseScale, attackMinScale, attackTime / attackAnimationInterval));
        else
        {
            float progress = attackTime % attackAnimationInterval;
            if (progress > attackAnimationInterval / 2)
                progress = attackAnimationInterval - progress;
            ScaleOnFixedZAxis(activeNose, Vector3.Lerp(attackMinScale, attackMaxScale, progress / (attackAnimationInterval / 2)));
        }
    }


}
