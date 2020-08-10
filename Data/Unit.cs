using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unit
{
    public UnitType type { get; protected set; }
    public Tile tile { get; protected set; }
    public Player owner { get; protected set; }
    public int moves { get; protected set; }

    Tile target;
    Queue<Tile> currentPath;

    public static event Action<Unit, Tile, Tile> onUnitMoved;
    public static event Action<Unit> onUnitDestroyed;

    public event Action onTurnFinished;

    public Unit(UnitType type, Tile tile, Player owner)
    {
        this.type = type;
        SetTile(tile);

        this.owner = owner;
        owner.AddUnit(this);
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

        while(tile != target && moves > 0)
        {
            if (currentPath == null || currentPath.Count == 0) GeneratePath();

            Tile nextTile = currentPath.Dequeue();

            if (!nextTile.Interact(this)) GeneratePath();
        }

        if (moves == 0) onTurnFinished?.Invoke();
    }

    /// <summary>
    /// Generate new path to the target.
    /// </summary>
    void GeneratePath()
    {
        currentPath = World.GetPath(this, target);
        currentPath.Dequeue();
    }

    /// <summary>
    /// Attack the given unit.
    /// </summary>
    /// <param name="enemy"></param>
    public void Battle(Unit enemy)
    {
        if (enemy.owner == owner) return;

        if (UnityEngine.Random.Range(0, 2) == 0) Destroy();
        else
        {
            Tile tile = enemy.tile;
            enemy.Destroy();
            SetTile(tile);
            moves = 0;
        }
    }

    /// <summary>
    /// Destroy this unit.
    /// </summary>
    void Destroy()
    {
        owner.RemoveUnit(this);
        tile.SetUnit(null);
        moves = 0;
        onUnitDestroyed?.Invoke(this);
    }

    /// <summary>
    /// Called on the start of the owners turn.
    /// </summary>
    public void Refresh()
    {
        moves = 1;
    }

    public override string ToString()
    {
        return owner + " " + type.name;
    }
}
