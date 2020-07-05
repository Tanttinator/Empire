using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    static void GenerateWorld()
    {
        tiles = new Tile[Width, Height];

        for(int x = 0; x < Width; x++)
        {
            for(int y = 0; y < Height; y++)
            {
                tiles[x, y] = new Tile(new Coords(x, y), (Random.value < 0.5f? instance.grassland : instance.water), instance);
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
    /// Are the given coordiantes within this worlds dimensions?
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool ValidCoords(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    private void Start()
    {
        GenerateWorld();
        WorldGraphics.InitTiles();
        UnitController.SpawnUnit(UnitController.Units[0], GetTile(Width / 2, Height / 2));
    }

    private void Awake()
    {
        instance = this;
    }
}
