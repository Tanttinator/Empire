using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible for the graphical representation of the world.
/// </summary>
public class WorldGraphics : MonoBehaviour
{

    [SerializeField] TileGraphics tileObject = default;
    [SerializeField] GroundSpriteData[] groundSprites = default;
    static TileGraphics[,] tiles;

    static WorldGraphics instance;

    /// <summary>
    /// Create all tile objects.
    /// </summary>
    public static void InitTiles(int width, int height)
    {
        tiles = new TileGraphics[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y] = Instantiate(instance.tileObject.gameObject, GetTilePosition(new Coords(x, y)), Quaternion.identity, instance.transform).GetComponent<TileGraphics>();
            }
        }
    }

    /// <summary>
    /// Refresh the graphics of the given tile.
    /// </summary>
    /// <param name="coords"></param>
    /// <param name="data"></param>
    void RefreshTile(Coords coords, TileData data)
    {
        tiles[coords.x, coords.y].Refresh(data);
    }

    /// <summary>
    /// Find a sprite for the ground type in the registry.
    /// </summary>
    /// <param name="ground"></param>
    /// <returns></returns>
    public static Sprite GetGroundSprite(Ground ground)
    {
        foreach(GroundSpriteData data in instance.groundSprites)
        {
            if (data.ground == ground) return data.sprite;
        }
        Debug.LogError("No sprite found for ground of type: " + ground.name);
        return null;
    }

    /// <summary>
    /// Returns the graphics at the given coordinates.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static TileGraphics GetTileGraphics(int x, int y)
    {
        if (!World.ValidCoords(x, y)) return null;
        return tiles[x, y];
    }

    /// <summary>
    /// Returns the graphics at the given coordinates.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static TileGraphics GetTileGraphics(Coords c)
    {
        if (!World.ValidCoords(c.x, c.y)) return null;
        return tiles[c.x, c.y];
    }

    /// <summary>
    /// Returns the coords of the tile whose graphics overlap a point in the world.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public static Coords GetTileAtPoint(Vector2 point)
    {
        return new Vector2(point.x - instance.transform.position.x, point.y - instance.transform.position.y);
    }

    /// <summary>
    /// Returns the coordinates for the position of the given tile in the world graphics.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public static Vector3 GetTilePosition(Coords coords)
    {
        return new Vector3(coords.x, coords.y, 0f);
    }

    private void Awake()
    {
        instance = this;
        GameState.onTileUpdated += RefreshTile;
    }

    private void OnDisable()
    {
        GameState.onTileUpdated -= RefreshTile;
    }

}

[System.Serializable]
public struct GroundSpriteData
{
    public Ground ground;
    public Sprite sprite;
}
