using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth : IDamageable
{
    
    void HealDamage(int damage);

    void AddDeathListener(IDeathListener listener);

}
