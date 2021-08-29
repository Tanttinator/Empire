using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PerlinNoise;
using System.Linq;

namespace Server
{
    /// <summary>
    /// Stores data for the current world.
    /// </summary>
    public class World : MonoBehaviour
    {
        [Header("World Parameters")]
        [SerializeField] int width = 10;
        [SerializeField] int height = 10;
        [SerializeField] Settings islandSettings = default;
        [SerializeField, Range(0f, 1f)] float waterLevel = 0.5f;
        [SerializeField] float landAmount = 0.1f;
        [SerializeField] Settings mountainsSettings = default;
        [SerializeField, Range(0f, 1f)] float mountainsHeight = 0.5f;
        [SerializeField] Settings forestSettings = default;
        [SerializeField, Range(0f, 1f)] float forestAmount = 0.5f;
        [SerializeField, Range(0f, 1f)] float riverStartHeight = 0.5f;
        [SerializeField, Range(0f, 1f)] float riverFrequency = 0.1f;
        [SerializeField] int cityRadius = 2;
        [SerializeField] int cityDensity = 5;

        public static int Width => instance.width;
        public static int Height => instance.height;

        static Tile[,] tiles;
        static List<Island> islands;

        static World instance;

        #region Generation

        /// <summary>
        /// Generate tiles based on the worlds parametes.
        /// </summary>
        public static void GenerateWorld(Player[] players)
        {
            tiles = new Tile[Width, Height];

            int seed = Random.Range(-999999, 999999);
            Random.InitState(seed);

            float[,] heightmap = Generator.GenerateHeightmap(Width, Height, seed, instance.islandSettings, Vector2.zero);
            float[,] mountains = Generator.GenerateHeightmap(Width, Height, seed + 1, instance.mountainsSettings, Vector2.zero);
            float[,] forests = Generator.GenerateHeightmap(Width, Height, seed + 2, instance.forestSettings, Vector2.zero);

            //Generate Terrain.
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    float height = heightmap[x, y] * Falloff(x, y);
                    Tile tile = tiles[x, y] = new Tile(new Coords(x, y), height > instance.waterLevel);
                }
            }

            //Generate Rivers
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Tile tile = GetTile(x, y);
                    if (tile.feature == null && heightmap[x, y] > instance.riverStartHeight && heightmap[x, y] < instance.mountainsHeight && Random.Range(0f, 1f) < instance.riverFrequency)
                    {
                        GenerateRiver(tile, null, heightmap);
                    }
                }
            }

            //Generate Features
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Tile tile = GetTile(x, y);

                    if (tile.land && tile.feature == null)
                    {
                        if (heightmap[x, y] > instance.mountainsHeight) tile.SetFeature(Feature.mountains);
                        else if (forests[x, y] < instance.forestAmount) tile.SetFeature(Feature.forest);
                    }
                }
            }

            islands = new List<Island>();

            //Create Islands.
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Tile tile = GetTile(x, y);
                    if (tile.land && tile.island == null)
                    {
                        Island island = new Island();
                        islands.Add(island);
                        FloodfillIsland(tile, island);
                    }
                }
            }

            islands = islands.OrderBy(i => i.Area).Reverse().ToList();

            //Generate cities
            for (int i = 0; i < Width * Height / Mathf.Pow(instance.cityRadius * 2 + 1, 2); i++)
            {
                for (int t = 0; t < instance.cityDensity; t++)
                {
                    Tile tile = GetRandomTile();

                    if (!tile.CanBuildStructure) continue;

                    Tile[] range = GetTilesInRange(tile, instance.cityRadius);

                    bool validSpot = true;

                    foreach (Tile other in range)
                    {
                        if (other.structure is City) validSpot = false;
                    }

                    if (!validSpot) continue;

                    Structure.CreateStructure(new City(), tile, GameController.neutral);
                    break;
                }
            }

            //Assign starting cities to all players.
            foreach (Player player in players)
            {
                float bestScore = 0f;
                City bestCity = null;

                foreach (City city in City.cities)
                {
                    float score = CalculateStartingCityScore(city);

                    if (score > bestScore || bestCity == null)
                    {
                        bestScore = score;
                        bestCity = city;
                    }
                }

                bestCity.efficiency = 100;
                bestCity.SetOwner(player);
                Unit.CreateUnit(Unit.infantry, bestCity.tile, player);
            }
        }

        /// <summary>
        /// Generate a river recursively.
        /// </summary>
        /// <param name="tile"></param>
        static void GenerateRiver(Tile tile, Direction cameFrom, float[,] heightmap)
        {
            if (tile == null || !tile.land || tile.feature == Feature.river) return;

            tile.SetFeature(Feature.river);

            Tile nextTile = null;
            Direction nextDir = null;
            float nextHeight = Mathf.Infinity;

            foreach (Direction dir in new Direction[] { Direction.NORTH, Direction.EAST, Direction.SOUTH, Direction.WEST })
            {
                if (dir == cameFrom) continue;

                Tile other = GetTile(tile.coords.Neighbor(dir));
                if (other != null)
                {
                    float height = heightmap[other.coords.x, other.coords.y];
                    if (height < nextHeight)
                    {
                        nextTile = other;
                        nextDir = dir;
                        nextHeight = height;
                    }
                }
            }

            GenerateRiver(nextTile, nextDir, heightmap);

        }

        /// <summary>
        /// Assign this tile and its neighbors recursively to the given island.
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="island"></param>
        static void FloodfillIsland(Tile tile, Island island)
        {
            Stack<Tile> tiles = new Stack<Tile>();
            tiles.Push(tile);

            while (tiles.Count > 0)
            {
                Tile newTile = tiles.Pop();

                if (newTile.land && newTile.island == null)
                {
                    island.AddTile(newTile);

                    foreach (Tile neighbor in GetNeighbors(newTile)) tiles.Push(neighbor);
                }
            }
        }

        /// <summary>
        /// Use falloff to reduce land around the edge of the map.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        static float Falloff(int x, int y)
        {
            float a = 3f;

            float valueX = Mathf.Abs(x * 2f / Width - 1);
            float valueY = Mathf.Abs(y * 2f / Height - 1);
            float dist = Mathf.Max(valueX, valueY);
            float value = Mathf.Max(0, 1 - dist);

            return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(instance.landAmount - instance.landAmount * value, a));
        }

        /// <summary>
        /// Calculates the score for the given city used to assign starting cities to players.
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        static float CalculateStartingCityScore(City city)
        {
            float closestPlayer = Mathf.Sqrt(Width * Width + Height * Height);

            foreach (City other in City.cities)
            {
                if (other.owner != GameController.neutral)
                {
                    float dist = Vector2.Distance(city.tile.coords, other.tile.coords);
                    if (dist < closestPlayer) closestPlayer = dist;
                }
            }

            return (city.tile.IsCoastal ? 1f : 0f) * (city.owner == GameController.neutral ? 1f : 0f) * city.tile.island.Area * closestPlayer;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Try to get a tile at the given coordinates.
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        public static Tile GetTile(Coords coords)
        {
            return GetTile(coords.x, coords.y);
        }

        /// <summary>
        /// Try to get a tile at the given coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Tile GetTile(int x, int y)
        {
            if (!ValidCoords(x, y)) return null;
            return tiles[x, y];
        }

        /// <summary>
        /// Get all neighboring tiles of a tile.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public static Tile[] GetNeighbors(Tile tile)
        {
            List<Tile> tiles = new List<Tile>();
            foreach (Direction dir in Direction.directions)
            {
                Tile neighbor = GetNeighbor(tile, dir);
                if (neighbor != null) tiles.Add(neighbor);
            }
            return tiles.ToArray();
        }

        /// <summary>
        /// Return the neighbor of the given tile in a direction.
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static Tile GetNeighbor(Tile tile, Direction dir)
        {
            return GetTile(tile.coords.Neighbor(dir));
        }

        /// <summary>
        /// Return all tiles within a range of the center tile.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static Tile[] GetTilesInRange(Tile center, int range)
        {
            int centerX = center.coords.x;
            int centerY = center.coords.y;

            List<Tile> tiles = new List<Tile>();

            for (int x = centerX - range; x <= centerX + range; x++)
            {
                for (int y = centerY - range; y <= centerY + range; y++)
                {
                    Tile tile = GetTile(x, y);
                    if (tile != null) tiles.Add(tile);
                }
            }

            return tiles.ToArray();
        }

        /// <summary>
        /// Returns a random tile in the world.
        /// </summary>
        /// <returns></returns>
        public static Tile GetRandomTile()
        {
            return GetTile(Random.Range(0, Width), Random.Range(0, Height));
        }

        /// <summary>
        /// Are the given coordiantes within this worlds dimensions?
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool ValidCoords(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        /// <summary>
        /// Are the given coordinates within this worlds dimensions?
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        public static bool ValidCoords(Coords coords)
        {
            return coords.x >= 0 && coords.x < Width && coords.y >= 0 && coords.y < Height;
        }

        /// <summary>
        /// Find the shortest path to the target for the given unit.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static Queue<Tile> GetPath(Unit unit, Tile target)
        {
            return AStar.AStar.GeneratePath<Tile>(unit.tile, target, unit);
        }

        #endregion

        private void Awake()
        {
            instance = this;
        }
    }
}