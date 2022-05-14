using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{

    public Collider damageCollider;

    public int damage = 10;

    private void Awake()
    {
        damageCollider.gameObject.SetActive(true);
        damageCollider.isTrigger = true;
        DisableDamageCollider();
    }

    public void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    public void DisableDamageCollider()
    {
        damageCollider.enabled = false;
    }


}
