using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDeathListener
{

    void OnDeath(IHealth died);

}
