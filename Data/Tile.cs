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

    /// <summary>
    /// Called when the given unit tries to move onto this tile.
    /// </summary>
    /// <param name="unit"></param>
    public bool Interact(Unit unit)
    {
        if (this.unit == null && CanEnter(unit))
        {
            return unit.Move(this);
        }

        if (this.unit.owner != unit.owner)
        {
            unit.Battle(this.unit);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Can the given unit enter this tile?
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    bool CanEnter(Unit unit)
    {
        return this.unit == null || this.unit.owner != unit.owner;
    }

    /// <summary>
    /// How many movement points does it cost for the given unit to enter this tile?
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public int MovementCost(Unit unit)
    {
        return 1;
    }

    float INode.EntryCost(object agent, INode from)
    {
        return MovementCost(agent as Unit);
    }

    bool INode.CanEnter(object agent, INode from)
    {
        return CanEnter(agent as Unit);
    }
}
