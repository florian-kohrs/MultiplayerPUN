using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public string playerName;

    protected bool isAlive = true;

    public bool IsAlive
    {
        get { return isAlive;}
        set
        {
            if(!hasReachedTarget)
            {
                isAlive = value;
                
            }    
        }
    }

    protected bool hasReachedTarget;

    public void ResetValues()
    {
        hasReachedTarget = false;
        isAlive = true;
    }

    public bool HasReachedTarget
    {
        get { return hasReachedTarget;}
        set
        {
            if(isAlive)
            {
                HasReachedTarget = value;
                Broadcast(/*Died?*/);
            }
        }
    }

    protected void Broadcast()
    {

    }

}
