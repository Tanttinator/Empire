using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds data for one tile in the map.
/// </summary>
public class Tile
{
    public Coords coords { get; protected set; }
    World world;

    public Tile(Coords coords, World world)
    {
        this.coords = coords;
        this.world = world;
    }
}
