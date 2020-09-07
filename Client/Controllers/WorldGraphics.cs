using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible for the graphical representation of the world.
/// </summary>
public class WorldGraphics : MonoBehaviour
{

    [SerializeField] TileGraphics tileObject = default;
    [SerializeField] SpriteProvider landSprite = default;
    [SerializeField] SpriteProvider waterSprite = default;
    [SerializeField] StructureSpriteData[] structureSprites = default;
    [SerializeField] FeatureSpriteData[] featureSprites = default;
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

        ClientController.Camera.SetConstraints(0f, 0f, width, height);
    }

    #region Accessors

    /// <summary>
    /// Find a sprite for the ground type from the registry.
    /// </summary>
    /// <param name="ground"></param>
    /// <returns></returns>
    public static SpriteOrientationData GetGroundSprite(TileData tile)
    {
        SpriteProvider sprite;
        if (tile.land) sprite = instance.landSprite;
        else sprite = instance.waterSprite;

        return sprite.GetSprite(tile.landConnections[0], tile.landConnections[1], tile.landConnections[2], tile.landConnections[3]);
    }

    /// <summary>
    /// Find a sprite for the structure type from the registry.
    /// </summary>
    /// <param name="structure"></param>
    /// <returns></returns>
    public static Sprite GetStructureSprite(StructureType structure)
    {
        foreach (StructureSpriteData data in instance.structureSprites)
        {
            if (data.structure == structure) return data.sprite;
        }
        Debug.LogError("No sprite found for structure of type: " + structure);
        return null;
    }

    /// <summary>
    /// Find a sprite for the feature from the registry.
    /// </summary>
    /// <param name="feature"></param>
    /// <returns></returns>
    public static SpriteOrientationData GetFeatureSprite(TileData tile)
    {
        foreach(FeatureSpriteData data in instance.featureSprites)
        {
            if (data.feature == tile.feature) return data.sprite.GetSprite(tile.featureConnections[0], tile.featureConnections[1], tile.featureConnections[2], tile.featureConnections[3]);
        }
        Debug.LogError("No sprite found for feature of type: " + tile.feature);
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

    #endregion

    private void Awake()
    {
        instance = this;
    }

}

[System.Serializable]
public struct StructureSpriteData
{
    public StructureType structure;
    public Sprite sprite;
}

[System.Serializable]
public struct FeatureSpriteData
{
    public string feature;
    public SpriteProvider sprite;
}
