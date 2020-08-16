using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameState : MonoBehaviour
{
    static TileData[,] tiles;

    public static event Action<Coords, TileData> onTileUpdated;

    /// <summary>
    /// Initialize tiles array.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public static void Init(int width, int height)
    {
        tiles = new TileData[width, height];
    }

    /// <summary>
    /// Update a single tile.
    /// </summary>
    /// <param name="coords"></param>
    /// <param name="data"></param>
    static void UpdateTile(Coords coords, TileData data)
    {
        tiles[coords.x, coords.y] = data;
        onTileUpdated?.Invoke(coords, data);
    }

    /// <summary>
    /// Update all tiles.
    /// </summary>
    /// <param name="tiles"></param>
    public static void UpdateTiles(TileData[,] tiles)
    {
        for(int x = 0; x < tiles.GetLength(0); x++)
        {
            for(int y = 0; y < tiles.GetLength(1); y++)
            {
                UpdateTile(new Coords(x, y), tiles[x, y]);
            }
        }
    }

    /// <summary>
    /// Place a unit on a tile.
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="unit"></param>
    /// <param name="unitColor"></param>
    public static void PlaceUnit(Coords tile, UnitType unit, Color unitColor)
    {
        TileData data = tiles[tile.x, tile.y];
        data.unit = unit;
        data.unitColor = unitColor;
        UpdateTile(tile, data);
    }

    /// <summary>
    /// Remove a unit from a tile.
    /// </summary>
    /// <param name="tile"></param>
    public static void RemoveUnit(Coords tile)
    {
        TileData data = tiles[tile.x, tile.y];
        data.unit = null;
        UpdateTile(tile, data);
    }

    /// <summary>
    /// Move a unit on the map.
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public static void MoveUnit(UnitType unit, Color unitColor, Coords from, Coords to)
    {
        RemoveUnit(from);

        PlaceUnit(to, unit, unitColor);
    }
}
