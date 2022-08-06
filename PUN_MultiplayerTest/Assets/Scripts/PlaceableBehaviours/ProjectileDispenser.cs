using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileDispenser : BasePlaceableBehaviours
{

    public GameObject projectile;

    protected GameObject projectileInstance;

    public float dispenserCooldown = 3;

    public float dispensePower = 5;

    protected void Start()
    {
        StopAllCoroutines();
        StartCoroutine(DispenseCoroutine());
    }

    public abstract GameObject Fire();

    protected IEnumerator DispenseCoroutine()
    {
        yield return new WaitForSeconds(dispenserCooldown);
        projectileInstance = Fire();
        IHasPlacedById[] placed = projectileInstance.GetComponentsInChildren<IHasPlacedById>();
        foreach (var item in placed)
        {
            item.PlacedByPlayerID = PlacedByPlayerID;
        }
        yield return DispenseCoroutine();
    }

}
