using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCubeOccupationObject : MapOccupationObject
{

    public Sprite cubeSprite;

    public Color spriteColor;

    public override void ApplyToObject(GameObject prefabInstace)
    {
        SpriteRenderer renderer = prefabInstace.GetComponentInChildren<SpriteRenderer>();
        renderer.color = spriteColor;
        renderer.sprite = cubeSprite;
    }

}
