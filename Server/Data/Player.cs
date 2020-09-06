using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player
{
    public string name { get; protected set; }
    public Color color { get; protected set; }

    List<Unit> units = new List<Unit>();
    public Unit[] Units => units.ToArray();

    //TODO: cache seen tiles when they are updated.
    public TileData[,] SeenTiles
    {
        get
        {
            TileData[,] tiles = new TileData[World.Width, World.Height];

            for(int x = 0; x < World.Width; x++)
            {
                for(int y = 0; y < World.Height; y++)
                {
                    tiles[x, y] = World.GetTile(x, y).VisibleState(this);
                }
            }

            return tiles;
        }
    }

    public event Action onTurnStarted;

    public Player(string name, Color color)
    {
        this.name = name;
        this.color = color;
    }

    /// <summary>
    /// Called when this players turn starts.
    /// </summary>
    public void StartTurn()
    {
        onTurnStarted?.Invoke();
        foreach (Unit unit in units)
        {
            unit.Refresh();
        }
    }

    /// <summary>
    /// Set all tiles visible to this player.
    /// </summary>
    public void RevealMap()
    {
        for(int x = 0; x < World.Width; x++)
        {
            for(int y = 0; y < World.Height; y++)
            {
                World.GetTile(x, y).Reveal(this);
            }
        }
    }

    /// <summary>
    /// Add a unit for this player.
    /// </summary>
    /// <param name="unit"></param>
    public void AddUnit(Unit unit)
    {
        units.Add(unit);
    }

    /// <summary>
    /// Remove a unit from this player.
    /// </summary>
    /// <param name="unit"></param>
    public void RemoveUnit(Unit unit)
    {
        units.Remove(unit);
    }

    public override string ToString()
    {
        return name;
    }
}
