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
                gfx.SetGround(World.GetTile(x, y).ground);
            }
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
