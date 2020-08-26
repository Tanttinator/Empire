using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteOrientationData
{
    public Sprite sprite;
    public float rotation;

    public SpriteOrientationData(Sprite sprite, float rotation)
    {
        this.sprite = sprite;
        this.rotation = rotation;
    }
}
