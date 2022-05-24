using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoseShooter : PunLocalBehaviour
{

    public GameObject nosePrefab;

    public Transform activeNose;

    protected float noseGrowTime = 0.5f;

    public Vector3 defaultNoseScale;

    protected float attackAnimationInterval = 1f;

    public Vector3 attackMaxScale;
    public Vector3 attackMinScale;

    protected float attackPower = 40;

    protected float attackTime;

    protected bool canAttack;

    [SerializeField]
    protected CameraController cameraController;

    protected override void OnStart()
    {
        GameManager.InputHandler.RegisterEvent(i => i.PlayerActions.DefaultAttack.performed += delegate { enabled = true/*canAttack*/;  }); 
        GameManager.InputHandler.RegisterEvent(i => i.PlayerActions.DefaultAttack.canceled += delegate { enabled = false; Attack(); });
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
        canAttack = true;
    }

    protected void ScaleOnFixedZAxis(Transform t, Vector3 newScale)
    {
        float zDiff = newScale.z - t.localScale.z;
        t.transform.localPosition += new Vector3(0, 0, zDiff / 2);
        t.transform.localScale = newScale;
    }

    protected void Attack()
    {
        ShootNose();
        StartCoroutine(GrowNose());
        attackTime = 0;
        canAttack = false;
    }

    protected void ShootNose()
    {
        GameObject nose;
        Transform noseProjectileOrientation = GetNoseOrientationTransform();
        if (PhotonNetwork.IsConnected)
        {
            nose = PhotonNetwork.Instantiate(nosePrefab.name, activeNose.position + activeNose.forward * 0.2f, noseProjectileOrientation.rotation);
        }
        else
        {
            nose = Instantiate(nosePrefab);
            nose.transform.position = activeNose.position + activeNose.forward * 0.2f;
            nose.transform.rotation = noseProjectileOrientation.rotation;
        }
        nose.transform.localScale = activeNose.localScale;
        Vector3 force = noseProjectileOrientation.forward * (20 + Mathf.InverseLerp(0, attackAnimationInterval, attackTime) * attackPower);
        nose.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
    }

    protected Transform GetNoseOrientationTransform()
    {
        if (cameraController.IsInFPP)
            return Camera.main.transform;
        else
            return transform;
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
