using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AStar;

/// <summary>
/// Holds data for one tile in the map.
/// </summary>
public class Tile : INode
{
    public Coords coords { get; protected set; }
    public Ground ground { get; protected set; }
    public Unit unit { get; protected set; }

    AStar.Vector2 INode.Position => coords;

    INode[] INode.Neighbors => World.GetNeighbors(this);

    public static event Action<Tile, Unit> onTileUnitSet;

    public Tile(Coords coords, Ground ground)
    {
        this.coords = coords;
        this.ground = ground;
    }

    /// <summary>
    /// Place the unit on this tile.
    /// </summary>
    /// <param name="unit"></param>
    public void SetUnit(Unit unit)
    {
        this.unit = unit;
        onTileUnitSet?.Invoke(this, unit);
    }

    float INode.EntryCost(object agent, INode from)
    {
        return 1f;
    }

    bool INode.CanEnter(object agent, INode from)
    {
        return true;
    }
}
