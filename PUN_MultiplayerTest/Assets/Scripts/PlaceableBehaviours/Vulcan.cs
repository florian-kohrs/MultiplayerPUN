using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vulcan : ProjectileDispenser
{

    public float randomDirectionOffset = 30;
    public float powerVariation = 2;

    protected HashSet<GameObject> shotProjectiles = new HashSet<GameObject>();

    [SerializeField]
    protected Transform spawnPosition;

    protected float GetNextDispensePower()
    {
        float r = Random.Range(0,1f);
        float power = Mathf.Sin(Mathf.PI * 2 * r) * 1.2f;
        power = Mathf.Clamp(power, -1, 1);
        return dispensePower + powerVariation * power;
    }

    public override void ResetOnNewRound()
    {
        foreach (GameObject g in shotProjectiles)
            Destroy(g);
        shotProjectiles.Clear();
    }

    public Vector2 GetDispenseDirection()
    {
        float angle = randomDirectionOffset * Random.Range(-1, 1f);
        return new Vector2(transform.up.x, transform.up.y).RotateVector(Mathf.Deg2Rad * angle);
    }

    protected Vector2 GetDispenseForce()
    {
        return GetDispenseDirection() * GetNextDispensePower();
    }

    public override GameObject Fire()
    {
        projectileInstance = Instantiate(projectile);
        projectileInstance.transform.position = spawnPosition.position;
        projectileInstance.GetComponent<Rigidbody2D>().AddForce(GetDispenseForce(), ForceMode2D.Impulse);
        shotProjectiles.Add(projectileInstance);

        projectileInstance.GetComponent<IOnDestroyCallback>().OnDestroyedListener(RemoveProjectile);
        return projectileInstance;
    }

    protected void RemoveProjectile(GameObject g)
    {
        shotProjectiles.Remove(g);
    }

}
