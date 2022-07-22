using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IButtonInputCallBack<T>
{

    protected void OnStarted();
    protected void OnPerformed();
    protected void OnCancelled();

}
