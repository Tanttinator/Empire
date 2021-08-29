using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public abstract class SpriteProvider : ScriptableObject
    {
        public abstract SpriteOrientationData GetSprite(bool connectNorth, bool connectEast, bool connectSouth, bool connectWest);
    }
}