using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sprite Provider/Single")]
public class SingleSpriteProvider : SpriteProvider
{
    public Sprite sprite = default;

    public override Sprite GetSprite(bool connectNorth, bool connectEast, bool connectSouth, bool connectWest)
    {
        return sprite;
    }
}
