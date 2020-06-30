using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unit
{
    public UnitType type { get; protected set; }
    public Tile tile { get; protected set; }

    public Unit(UnitType type, Tile tile)
    {
        this.type = type;
        SetTile(tile);
    }

    /// <summary>
    /// Place this unit on the given tile.
    /// </summary>
    /// <param name="tile"></param>
    public void SetTile(Tile tile)
    {
        this.tile?.SetUnit(null);
        this.tile = tile;
        tile.SetUnit(this);
    }
}
