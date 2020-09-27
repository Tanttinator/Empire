using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

namespace Client
{
    public static class GameState
    {
        static int width;
        static int height;
        static TileData[,] tiles;

        public static void UpdateTile(TileData data)
        {
            Coords coords = data.coords;
            tiles[coords.x, coords.y] = data;
            RefreshTile(coords);
        }

        public static void UpdateTiles(TileData[] data)
        {
            foreach (TileData tile in data) UpdateTile(tile);
        }

        public static void PlaceUnit(Coords tile, UnitData unit)
        {
            GetTile(tile).unit = unit;
            RefreshTile(tile);
        }

        public static void RemoveUnit(Coords tile)
        {
            GetTile(tile).unit = null;
            RefreshTile(tile);
        }

        public static void MoveUnit(UnitData unit, Coords from, Coords to)
        {
            PlaceUnit(to, unit);
            RemoveUnit(from);
        }

        public static void RefreshTile(Coords coords)
        {
            World.GetTileGraphics(coords).Refresh(tiles[coords.x, coords.y]);
        }

        public static TileData GetTile(Coords coords)
        {
            return tiles[coords.x, coords.y];
        }

        public static void Init(int width, int height)
        {
            GameState.width = width;
            GameState.height = height;
            tiles = new TileData[width, height];
        }
    }
}
