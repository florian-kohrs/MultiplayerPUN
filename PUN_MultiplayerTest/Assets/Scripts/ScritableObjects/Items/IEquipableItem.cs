using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquipableItem : IInstantiableItem
{

    Vector3 EquipLocalPosition { get; }

    Quaternion EquipLocalRotation { get; }
}
