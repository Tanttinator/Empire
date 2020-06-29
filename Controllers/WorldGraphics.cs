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
    public static void InitTiles()
    {
        int width = World.Width;
        int height = World.Height;

        tiles = new TileGraphics[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TileGraphics gfx = tiles[x, y] = Instantiate(instance.tileObject.gameObject, new Vector3(x, y, 0), Quaternion.identity, instance.transform).GetComponent<TileGraphics>();
                gfx.SetTile(World.GetTile(x, y));
            }
        }
    }

    /// <summary>
    /// Refresh the world graphics according to the current state of the game.
    /// </summary>
    public static void RefreshTiles()
    {
        for(int x = 0; x < World.Width; x++)
        {
            for(int y = 0; y < World.Height; y++)
            {
                RefreshTile(x, y);
            }
        }
    }

    /// <summary>
    /// Refresh graphics of a single tile.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public static void RefreshTile(int x, int y)
    {
        if (tiles[x, y] == null)
        {
            
        }
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
        return null;
    }

    private void Awake()
    {
        instance = this;
    }

}

[System.Serializable]
public struct GroundSpriteData
{
    public Ground ground;
    public Sprite sprite;
}
