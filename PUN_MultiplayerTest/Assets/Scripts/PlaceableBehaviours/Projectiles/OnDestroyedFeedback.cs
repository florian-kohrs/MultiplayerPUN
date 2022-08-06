using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDestroyedFeedback : MonoBehaviour, IOnDestroyCallback
{
    protected Action<GameObject> f;

    public void OnDestroyedListener(Action<GameObject> f)
    {
        this.f = f;
    }

    private void OnDestroy()
    {
        f?.Invoke(gameObject);
    }
}
