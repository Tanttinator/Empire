using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sprite Provider/Connected")]
public class ConnectedSpriteProvider : SpriteProvider
{
    [SerializeField] Sprite none = default;
    [SerializeField] Sprite north = default;
    [SerializeField] Sprite northEast = default;
    [SerializeField] Sprite northSouth = default;
    [SerializeField] Sprite northEastSouth = default;
    [SerializeField] Sprite northEastSouthWest = default;
    public override SpriteOrientationData GetSprite(bool connectNorth, bool connectEast, bool connectSouth, bool connectWest)
    {
        string connections = "";

        if (connectNorth)
            connections += "N";
        if (connectEast)
            connections += "E";
        if (connectSouth)
            connections += "S";
        if (connectWest)
            connections += "W";

        switch (connections)
        {
            case "": return new SpriteOrientationData(none, 0f);

            case "N": return new SpriteOrientationData(north, 0f);
            case "E": return new SpriteOrientationData(north, -90f);
            case "S": return new SpriteOrientationData(north, -180f);
            case "W": return new SpriteOrientationData(north, -270f);

            case "NE": return new SpriteOrientationData(northEast, 0f);
            case "ES": return new SpriteOrientationData(northEast, -90f);
            case "SW": return new SpriteOrientationData(northEast, -180f);
            case "NW": return new SpriteOrientationData(northEast, -270f);

            case "NS": return new SpriteOrientationData(northSouth, 0f);
            case "EW": return new SpriteOrientationData(northSouth, -180f);

            case "NES": return new SpriteOrientationData(northEastSouth, 0f);
            case "ESW": return new SpriteOrientationData(northEastSouth, -90f);
            case "NSW": return new SpriteOrientationData(northEastSouth, -180f);
            case "NEW": return new SpriteOrientationData(northEastSouth, -270f);

            default: return new SpriteOrientationData(northEastSouthWest, 0f);
        }
    }
}
