using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Common
{
    [System.Serializable]
    public class GameState
    {
        PlayerData[] players;
        TileData[,] tiles;
        Dictionary<int, UnitData> units;
        public UnitData[] Units => units.Values.ToArray();

        public GameState(int mapWidth, int mapHeight, PlayerData[] players)
        {
            this.players = players;
            tiles = new TileData[mapWidth, mapHeight];
            for(int x = 0; x < mapWidth; x++)
            {
                for(int y = 0; y < mapHeight; y++)
                {
                    tiles[x, y] = new TileData()
                    {
                        coords = new Coords(x, y),
                        discovered = false
                    };
                }
            }
            units = new Dictionary<int, UnitData>();
        }

        GameState(PlayerData[] players, TileData[,] tiles, Dictionary<int, UnitData> units)
        {
            this.players = players;
            this.tiles = tiles;
            this.units = units;
        }

        public void UpdateTile(TileData tile)
        {
            Coords coords = tile.coords;
            tiles[coords.x, coords.y] = tile;
        }

        public void UpdateUnit(UnitData unit)
        {
            units[unit.ID] = unit;
        }

        public void RemoveUnit(int unit)
        {
            units.Remove(unit);
        }

        public void UpdatePlayer(PlayerData player)
        {
            players[player.ID] = player;
        }

        public TileData GetTile(Coords coords)
        {
            return tiles[coords.x, coords.y];
        }

        public UnitData GetUnit(int ID)
        {
            if(units.ContainsKey(ID))
                return units[ID];
            return null;
        }

        public PlayerData GetPlayer(int ID)
        {
            return players[ID];
        }

        public GameState Clone()
        {
            return new GameState((PlayerData[])players.Clone(), (TileData[,])tiles.Clone(), units.ToDictionary(entry => entry.Key, entry => entry.Value));
        }
    }
}
