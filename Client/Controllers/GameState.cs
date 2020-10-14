using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

namespace Client
{
    public static class GameState
    {
        static Dictionary<int, PlayerData> players;
        static Dictionary<int, UnitData> units;
        static Dictionary<int, StructureData> structures;
        static TileData[,] tiles;

        #region Players

        public static void UpdatePlayer(PlayerData player)
        {
            players[player.ID] = player;
        }

        public static PlayerData GetPlayer(int ID)
        {
            if (!players.ContainsKey(ID)) return null;
            return players[ID];
        }

        #endregion

        #region Units

        public static void UpdateUnit(UnitData unit)
        {
            units[unit.ID] = unit;
        }

        public static UnitData GetUnit(int ID)
        {
            if (!units.ContainsKey(ID)) return null;
            return units[ID];
        }

        public static void PlaceUnit(Coords tile, int unit)
        {
            GetTile(tile).unit = unit;
            RefreshTile(tile);
        }

        public static void RemoveUnit(Coords tile)
        {
            GetTile(tile).unit = -1;
            RefreshTile(tile);
        }

        public static void MoveUnit(int unit, Coords from, Coords to)
        {
            PlaceUnit(to, unit);
            RemoveUnit(from);
        }

        #endregion

        #region Structures

        public static void UpdateStructure(StructureData structure)
        {
            structures[structure.ID] = structure;
        }

        public static StructureData GetStructure(int ID)
        {
            if (!structures.ContainsKey(ID)) return null;
            return structures[ID];
        }

        public static T GetStructure<T>(int ID) where T : StructureData
        {
            if (!structures.ContainsKey(ID)) return null;
            StructureData data = structures[ID];

            if (data == null || !(data is T)) return null;

            return (T)data;
        }

        #endregion

        #region Tiles

        public static void UpdateTile(TileData data)
        {
            Coords coords = data.coords;
            tiles[coords.x, coords.y] = data;
            RefreshTile(coords);
        }

        public static TileData GetTile(Coords coords)
        {
            return tiles[coords.x, coords.y];
        }

        public static void UpdateTiles(TileData[] data)
        {
            foreach (TileData tile in data) UpdateTile(tile);
        }

        static void RefreshTile(Coords coords)
        {
            World.GetTileGraphics(coords).Refresh(tiles[coords.x, coords.y]);
        }

        #endregion

        public static void Init(int width, int height, PlayerData[] players)
        {
            GameState.players = new Dictionary<int, PlayerData>();
            units = new Dictionary<int, UnitData>();
            structures = new Dictionary<int, StructureData>();
            tiles = new TileData[width, height];

            foreach (PlayerData player in players) UpdatePlayer(player);
        }
    }
}
