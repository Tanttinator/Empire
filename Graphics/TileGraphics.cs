using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible for graphical representation of a single tile.
/// </summary>
public class TileGraphics : MonoBehaviour
{
    [SerializeField] SpriteRenderer groundGfx = default;
    [SerializeField] UnitGraphics unitGfx = default;

    /// <summary>
    /// Set the target tile.
    /// </summary>
    /// <param name="tile"></param>
    public void SetGround(Ground ground)
    {
        groundGfx.sprite = WorldGraphics.GetGroundSprite(ground);
    }

    /// <summary>
    /// Set the unit on this tile.
    /// </summary>
    /// <param name="unit"></param>
    public void SetUnit(Unit unit)
    {
        unitGfx.SetUnit(unit);
    }
}
