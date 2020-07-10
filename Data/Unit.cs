using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unit
{
    public UnitType type { get; protected set; }
    public Tile tile { get; protected set; }

    Tile target;
    Queue<Tile> currentPath;

    public static event Action<Unit, Tile, Tile> onUnitMoved;

    public Unit(UnitType type, Tile tile)
    {
        this.type = type;
        SetTile(tile);
    }

    /// <summary>
    /// Place this unit on the given tile.
    /// </summary>
    /// <param name="tile"></param>
    public bool SetTile(Tile tile)
    {
        Tile oldTile = this.tile;
        oldTile?.SetUnit(null);
        this.tile = tile;
        tile.SetUnit(this);
        onUnitMoved?.Invoke(this, oldTile, tile);
        return true;
    }

    /// <summary>
    /// Set the target tile for this unit to move to.
    /// </summary>
    /// <param name="tile"></param>
    public void SetTarget(Tile tile)
    {
        target = tile;
    }

    /// <summary>
    /// Order this unit to move towards it's target.
    /// </summary>
    public void MoveToTarget()
    {
        if (target == null) return;

        while(tile != target)
        {
            if (currentPath == null || currentPath.Count == 0) GeneratePath();

            Tile nextTile = currentPath.Dequeue();

            if (!SetTile(nextTile)) GeneratePath();
        }
    }

    /// <summary>
    /// Generate new path to the target.
    /// </summary>
    void GeneratePath()
    {
        currentPath = World.GetPath(this, target);
        currentPath.Dequeue();
    }
}
