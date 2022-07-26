using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInstantiableItem : IItem
{

    GameObject GetItemInstance(Transform parent);


}
