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
    public static event Action<Unit> onUnitCreated;
    public static event Action<Unit, Unit, Unit[]> onUnitsBattled;

    public Unit(UnitType type, Tile tile, Player owner)
    {
        this.type = type;
        this.owner = owner;

        SetTile(tile);

        owner.AddUnit(this);

        onUnitCreated?.Invoke(this);
    }

    /// <summary>
    /// Place this unit on the given tile.
    /// </summary>
    /// <param name="tile"></param>
    public void SetTile(Tile tile)
    {
        Tile oldTile = this.tile;
        oldTile?.SetUnit(null);
        this.tile = tile;
        tile.SetUnit(this);
    }

    /// <summary>
    /// Try to move to the given target.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool Move(Tile tile, bool forced = false)
    {
        if (moves <= 0 && !forced) return false;

        Tile oldTile = this.tile;
        SetTile(tile);
        moves -= tile.MovementCost(this);
        onUnitMoved?.Invoke(this, oldTile, this.tile);
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
    /// Tell this unit to execute the next queued action if one exists.
    /// </summary>
    public bool DoTurn()
    {
        if (moves <= 0) return true;

        if(target != null && target != tile)
        {
            if (currentPath == null || currentPath.Count == 0) GeneratePath();

            Tile nextTile = currentPath.Dequeue();

            if (!nextTile.Interact(this)) GeneratePath();
        }

        return false;
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

        if (UnityEngine.Random.Range(0, 2) == 0)
        {
            onUnitsBattled?.Invoke(this, enemy, new Unit[] { this });
            Destroy();
        }
        else
        {
            Tile tile = enemy.tile;
            onUnitsBattled?.Invoke(this, enemy, new Unit[] { enemy });
            enemy.Destroy();
            Move(tile, true);
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
        moves = type.movement;
    }

    public UnitData GetData()
    {
        return new UnitData()
        {
            unit = type,
            color = owner.color
        };
    }

    public override string ToString()
    {
        return owner + " " + type.name;
    }
}
