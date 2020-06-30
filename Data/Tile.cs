using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Holds data for one tile in the map.
/// </summary>
public class Tile
{
    public Coords coords { get; protected set; }
    public Ground ground { get; protected set; }
    public Unit unit { get; protected set; }

    public event Action<Unit> onUnitSet;

    World world;

    public Tile(Coords coords, Ground ground, World world)
    {
        this.coords = coords;
        this.ground = ground;
        this.world = world;
    }

    /// <summary>
    /// Place the unit on this tile.
    /// </summary>
    /// <param name="unit"></param>
    public void SetUnit(Unit unit)
    {
        this.unit = unit;
        onUnitSet?.Invoke(unit);
    }
}
