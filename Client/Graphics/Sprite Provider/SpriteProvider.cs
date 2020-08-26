using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpriteProvider : ScriptableObject
{
    public abstract SpriteOrientationData GetSprite(bool connectNorth, bool connectEast, bool connectSouth, bool connectWest);
}
