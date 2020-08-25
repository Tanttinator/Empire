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

    TileData[,] seenTiles;
    public TileData[,] SeenTiles => (TileData[,])seenTiles.Clone();

    public event Action onTurnStarted;

    public Player(string name, Color color)
    {
        this.name = name;
        this.color = color;
    }

    public void StartTurn()
    {
        foreach (Unit unit in units)
        {
            unit.Refresh();
        }
        onTurnStarted?.Invoke();
    }

    public void InitVision()
    {
        seenTiles = new TileData[World.Width, World.Height];
    }

    public void RevealMap()
    {
        for(int x = 0; x < World.Width; x++)
        {
            for(int y = 0; y < World.Height; y++)
            {
                seenTiles[x, y] = World.GetTile(x, y).GetData(this, seenTiles[x, y], true);
            }
        }
    }

    public void AddUnit(Unit unit)
    {
        units.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        units.Remove(unit);
    }

    public void RefreshTile(Tile tile)
    {
        seenTiles[tile.coords.x, tile.coords.y] = tile.GetData(this, seenTiles[tile.coords.x, tile.coords.y]);
    }

    public override string ToString()
    {
        return name;
    }
}
