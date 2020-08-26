using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PerlinNoise;
using System.Linq;

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
    [SerializeField] Settings mountainsSettings = default;
    [SerializeField, Range(0f, 1f)] float mountainsAmount = 0.5f;
    [SerializeField] Settings forestSettings = default;
    [SerializeField, Range(0f, 1f)] float forestAmount = 0.5f;
    [SerializeField] int tilesPerCity = 25;

    [Header("Terrain")]
    [SerializeField] Ground grassland = default;
    [SerializeField] Ground water = default;
    [SerializeField] Feature mountains = default;
    [SerializeField] Feature forest = default;

    public static Ground Grassland => instance.grassland;
    public static Ground Water => instance.water;

    public static int Width => instance.width;
    public static int Height => instance.height;

    static Tile[,] tiles;
    static List<Island> islands;

    static World instance;

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
        for(int x = 0; x < Width; x++)
        {
            for(int y = 0; y < Height; y++)
            {
                Ground ground = (heightmap[x, y] > instance.waterLevel && (x > 0 && x < Width - 1 && y > 0 && y < Height - 1) ? Grassland : Water);
                Tile tile = tiles[x, y] = new Tile(new Coords(x, y), ground);

                if (ground == Grassland)
                {
                    if (mountains[x, y] < instance.mountainsAmount) tile.SetFeature(instance.mountains);
                    else if (forests[x, y] < instance.forestAmount) tile.SetFeature(instance.forest);
                }
            }
        }

        islands = new List<Island>();

        //Create Islands.
        for(int x = 0; x < Width; x++)
        {
            for(int y = 0; y < Height; y++)
            {
                Tile tile = GetTile(x, y);
                if (tile.ground == Grassland && tile.island == null)
                {
                    Island island = new Island();
                    islands.Add(island);
                    FloodfillIsland(tile, island);
                }
            }
        }

        islands = islands.OrderBy(i => i.Area).Reverse().ToList();

        //Place each player on a separate island.
        for(int i = 0; i < players.Length; i++)
        {
            City city = new City();
            Tile tile = islands[i].GetCoastalCitySpot();
            Structure.CreateStructure(city, tile, players[i]);
            UnitController.SpawnUnit(UnitController.Units[0], tile, players[i]);
        }

        //Generate Neutral Cities.
        foreach(Island island in islands)
        {
            int numCities = island.Area / instance.tilesPerCity + Random.Range(-1, 2);
            numCities = Mathf.Clamp(numCities, 0, island.Area);

            for(int i = 0; i < numCities; i++)
            {
                Tile tile = island.GetOptimalCitySpot();

                if (tile == null) break;

                City city = new City();
                Structure.CreateStructure(city, tile, GameController.neutral);
            }
        }
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

        while(tiles.Count > 0)
        {
            Tile newTile = tiles.Pop();

            if(newTile.ground == Grassland && newTile.island == null)
            {
                island.AddTile(newTile);

                foreach (Tile neighbor in GetNeighbors(newTile)) tiles.Push(neighbor);
            }
        }
    }

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
        foreach(Direction dir in Direction.directions)
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

    private void Awake()
    {
        instance = this;
    }
}
