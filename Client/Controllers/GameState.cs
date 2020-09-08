using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public static class GameState
    {
        static int width;
        static int height;
        static TileData[,] tiles;

        public static void UpdateTile(int x, int y, TileData data)
        {
            tiles[x, y] = data;
            RefreshTile(x, y);
        }

        public static void UpdateTiles(TileData[,] data)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    UpdateTile(x, y, data[x, y]);
                }
            }
        }

        public static void PlaceUnit(Coords tile, UnitData unit)
        {
            GetTile(tile).unit = unit;
            RefreshTile(tile.x, tile.y);
        }

        public static void RemoveUnit(Coords tile)
        {
            GetTile(tile).unit = null;
            RefreshTile(tile.x, tile.y);
        }

        public static void MoveUnit(UnitData unit, Coords from, Coords to)
        {
            PlaceUnit(to, unit);
            RemoveUnit(from);
        }

        public static void RefreshTile(int x, int y)
        {
            World.GetTileGraphics(x, y).Refresh(tiles[x, y]);
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
