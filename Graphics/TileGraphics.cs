using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible for graphical representation of a single tile.
/// </summary>
public class TileGraphics : MonoBehaviour
{
    Tile tile;

    [SerializeField] SpriteRenderer groundGfx = default;
    [SerializeField] UnitGraphics unitGfx = default;

    /// <summary>
    /// Set the target tile.
    /// </summary>
    /// <param name="tile"></param>
    public void SetTile(Tile tile)
    {
        this.tile = tile;
        groundGfx.sprite = WorldGraphics.GetGroundSprite(tile.ground);
        tile.onUnitSet += (u) => Refresh();
    }

    /// <summary>
    /// Refresh the graphics.
    /// </summary>
    public void Refresh()
    {
        unitGfx.SetUnit(tile.unit);
    }
}
