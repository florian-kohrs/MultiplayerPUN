using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableStuff : BaseHealth
{

    public AudioClip onHitPlay;

    public AudioClip onDestroyPlay;

    protected override void OnTakeNonLethalDamage()
    {
        AudioSource.PlayClipAtPoint(onHitPlay, transform.position);
    }

    protected override void OnEntityDied()
    {
        AudioSource.PlayClipAtPoint(onDestroyPlay, transform.position);
        Destroy(gameObject);
    }

}
