using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sprite Provider/Connected")]
public class ConnectedSpriteProvider : SpriteProvider
{
    public Sprite none, north, east, south, west, northEast, eastSouth, southWest, northWest, northSouth, eastWest, northEastSouth, eastSouthWest, northSouthWest, northEastWest, northEastSouthWest;
    public override Sprite GetSprite(bool connectNorth, bool connectEast, bool connectSouth, bool connectWest)
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
            case "": return none;
            case "N": return north;
            case "E": return east;
            case "S": return south;
            case "W": return west;
            case "NE": return northEast;
            case "ES": return eastSouth;
            case "SW": return southWest;
            case "NW": return northWest;
            case "NS": return northSouth;
            case "EW": return eastWest;
            case "NES": return northEastSouth;
            case "ESW": return eastSouthWest;
            case "NSW": return northSouthWest;
            case "NEW": return northEastWest;
            default: return northEastSouthWest;
        }
    }
}
