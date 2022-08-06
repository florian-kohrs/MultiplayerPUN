using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnDestroyCallback
{
    void OnDestroyedListener(Action<GameObject> f);

}