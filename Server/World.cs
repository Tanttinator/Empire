using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AStar;

/// <summary>
/// Stores data for the current world.
/// </summary>
public class World : MonoBehaviour
{
    [Header("World Parameters")]
    [SerializeField] int width = 10;
    [SerializeField] int height = 10;

    [Header("Terrain")]
    [SerializeField] Ground grassland = default;
    [SerializeField] Ground water = default;

    public static int Width => instance.width;
    public static int Height => instance.height;

    static Tile[,] tiles;

    static World instance;

    /// <summary>
    /// Generate tiles based on the worlds parametes.
    /// </summary>
    public static void GenerateWorld()
    {
        tiles = new Tile[Width, Height];

        for(int x = 0; x < Width; x++)
        {
            for(int y = 0; y < Height; y++)
            {
                tiles[x, y] = new Tile(new Coords(x, y), (Random.value < 0.5f? instance.grassland : instance.water));
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
