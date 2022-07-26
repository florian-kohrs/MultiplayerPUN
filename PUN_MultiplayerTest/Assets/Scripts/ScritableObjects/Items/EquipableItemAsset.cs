using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipableItemAsset : InstantiatableItem, IEquipableItem
{

    public Vector3 equipLocalPosition;
    public Vector3 equipLocalEulerAngle;
    public Vector3 localScale = new Vector3(1,1,1);

    public Vector3 EquipLocalPosition => equipLocalPosition;

    public Quaternion EquipLocalRotation => Quaternion.Euler(equipLocalEulerAngle);

    public override GameObject GetItemInstance(Transform parent)
    {
        GameObject result = base.GetItemInstance(parent);
        result.transform.localPosition = equipLocalPosition;
        result.transform.localEulerAngles = equipLocalEulerAngle;
        result.transform.localScale = localScale;
        return result;
    }

}
