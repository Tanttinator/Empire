using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public string name { get; protected set; }
    public Color color { get; protected set; }

    List<Unit> units = new List<Unit>();
    public Unit[] Units => units.ToArray();

    public TileData[,] seenTiles { get; protected set; }

    public Player(string name, Color color)
    {
        this.name = name;
        this.color = color;
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
        seenTiles[tile.coords.x, tile.coords.y] = tile.GetData();
    }

    public void RefreshVision()
    {
        seenTiles = new TileData[World.Width, World.Height];
        for (int x = 0; x < World.Width; x++)
        {
            for (int y = 0; y < World.Height; y++)
            {
                seenTiles[x, y] = World.GetTile(x, y).GetData();
            }
        }
    }

    public override string ToString()
    {
        return name;
    }
}
