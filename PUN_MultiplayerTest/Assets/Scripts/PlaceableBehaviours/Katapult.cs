using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katapult : ProjectileDispenser
{

    public Transform rotateAnchor;
    public Transform projectileParent;


    public float rotateSpeed = 360;
    public float rotateDegree = 100;

    protected Quaternion startRotation;

    protected new void Start()
    {
        CreateProjectile();
        startRotation = rotateAnchor.rotation;
        base.Start();
    }

    protected void CreateProjectile()
    {
        projectileInstance = Instantiate(projectile,projectileParent, false);
        projectileInstance.GetComponent<Rigidbody2D>().gravityScale = 0;
        IHasPlacedById placedBy = projectileInstance.GetComponentInChildren<IHasPlacedById>();
        if (placedBy != null)
        {
            placedBy.PlacedByPlayerID = PlacedByPlayerID;
        }
    }

    public override void Fire()
    {
        StartCoroutine(RotateAnimation());
    }

    protected IEnumerator RotateAnimation()
    {
        float rotateTime = 0;
        float rotateDuration = rotateDegree / rotateSpeed;
        do
        {
            yield return null;
            rotateTime += Time.deltaTime;
            float progress = rotateTime / rotateDuration;
            UpdateKatapult(progress);
        } while (rotateTime < rotateDuration);
        UpdateKatapult(1);
        ReleaseProjectile();
        rotateAnchor.rotation = startRotation;
        CreateProjectile();
    }

    protected void UpdateKatapult(float progress)
    {
        float rotation = Mathf.Lerp(0, rotateDegree, progress);
        rotateAnchor.rotation = startRotation * Quaternion.Euler(0, 0, -rotation * Mathf.Sign(transform.localScale.x));
    }

    protected void ReleaseProjectile()
    {
        if (projectileInstance == null)
            return;
        Rigidbody2D body = projectileInstance.GetComponent<Rigidbody2D>();
        projectileInstance.transform.parent = null;
        body.AddForce(projectileParent.up * dispensePower, ForceMode2D.Impulse);
        projectileInstance.GetComponentInChildren<CircleCollider2D>().enabled = true;
        body.gravityScale = 1;
        Destroy(projectileInstance, 10);
        projectileInstance = null;
    }

}
