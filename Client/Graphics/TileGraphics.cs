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
    [SerializeField] StructureGraphics structureGfx = default;

    public UnitGraphics Unit => unitGfx;

    /// <summary>
    /// Refresh graphics of this tile.
    /// </summary>
    /// <param name="data"></param>
    public void Refresh(TileData data)
    {
        groundGfx.sprite = WorldGraphics.GetGroundSprite(data.ground);
        unitGfx.SetUnit(data.unit);
        structureGfx.SetStructure(data.structure);
    }
}
