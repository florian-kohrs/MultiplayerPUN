using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideCollider : MonoBehaviour
{
    
    protected int triggerCount = 0;

    public bool IsTouching => triggerCount > 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        triggerCount++;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        triggerCount--;
    }

}
