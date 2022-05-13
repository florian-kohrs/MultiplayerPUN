using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAggressionTrigger : MonoBehaviour
{

    protected IAggressionTrigger target;

    protected HashSet<IAggressionTrigger> targets = new HashSet<IAggressionTrigger>();

    private void OnTriggerEnter(Collider other)
    {
        IAggressionTrigger target = other.GetComponent<IAggressionTrigger>();
        if (target == null)
            return;

        if(targets.Add(target) && this.target == null)
        {
            this.target = target;
            OnSetTarget();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IAggressionTrigger target = other.GetComponent<IAggressionTrigger>();
        if (target == null)
            return;

        targets.Remove(target);
        if (this.target == target)
        {
            Retarget();
        }
    }

    protected void Retarget()
    {
        if (targets.Count == 0)
        {
            target = null;
            OnLooseTarget();
        }
        else
        {
            IAggressionTrigger closestEnemy = null;
            float closest = float.MaxValue;
            foreach (var item in targets)
            {
                float sqrDist = Vector3.SqrMagnitude(transform.position - item.transform.position);
                if (sqrDist < closest)
                {
                    closestEnemy = item;
                    closest = sqrDist;
                }
            }
            target = closestEnemy;
            OnSwapTarget();
        }
    }

    protected virtual void OnSetTarget() { }

    protected virtual void OnSwapTarget() { }

    protected virtual void OnLooseTarget() { }

}
