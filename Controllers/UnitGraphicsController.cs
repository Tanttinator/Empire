using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGraphicsController : MonoBehaviour
{
    [SerializeField] UnitSpriteData[] unitSprites = default;

    static UnitGraphicsController instance;

    /// <summary>
    /// Find a sprite matching the given unit type in the registry.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public static Sprite GetUnitSprite(UnitType unit)
    {
        foreach(UnitSpriteData data in instance.unitSprites)
        {
            if (data.unit == unit) return data.sprite;
        }
        Debug.LogError("No sprite registered for unit of type: " + unit.name);
        return null;
    }

    /// <summary>
    /// Called when a unit of a tile changes.
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="unit"></param>
    void OnUnitTileSet(Tile tile, Unit unit)
    {
        WorldGraphics.GetTileGraphics(tile.coords.x, tile.coords.y).SetUnit(unit);
    }

    private void Awake()
    {
        instance = this;

        Tile.onTileUnitSet += OnUnitTileSet;
    }

    private void OnDisable()
    {
        Tile.onTileUnitSet -= OnUnitTileSet;
    }
}

[System.Serializable]
public struct UnitSpriteData
{
    public UnitType unit;
    public Sprite sprite;
}
