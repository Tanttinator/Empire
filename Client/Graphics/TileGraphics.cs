using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible for graphical representation of a single tile.
/// </summary>
public class TileGraphics : MonoBehaviour
{
    [SerializeField] SpriteRenderer groundGfx = default;
    [SerializeField] SpriteRenderer featureGfx = default;
    [SerializeField] UnitGraphics unitGfx = default;
    [SerializeField] StructureGraphics structureGfx = default;
    [SerializeField] SpriteRenderer fogOfWar = default;
    [SerializeField] SpriteRenderer unexplored = default;

    TileData state = null;

    public UnitGraphics Unit => unitGfx;

    /// <summary>
    /// Refresh graphics of this tile.
    /// </summary>
    public void Refresh()
    {
        if (state == null)
        {
            unexplored.enabled = true;
        }
        else
        {
            groundGfx.sprite = WorldGraphics.GetGroundSprite(state);
            if (state.feature == null) featureGfx.enabled = false;
            else
            {
                featureGfx.enabled = true;
                featureGfx.sprite = WorldGraphics.GetFeatureSprite(state);
            }
            unitGfx.SetUnit(state.unit);
            structureGfx.SetStructure(state.structure);
            fogOfWar.enabled = !state.visible;
            unexplored.enabled = false;
        }
    }

    /// <summary>
    /// Set the current state of this tile.
    /// </summary>
    /// <param name="data"></param>
    public void SetState(TileData state)
    {
        this.state = state;
        Refresh();
    }

    /// <summary>
    /// Place a unit on this tile.
    /// </summary>
    /// <param name="unit"></param>
    public void PlaceUnit(UnitData unit)
    {
        state.unit = unit;
        Refresh();
    }

    /// <summary>
    /// Remove the current unit from this tile.
    /// </summary>
    public void RemoveUnit()
    {
        state.unit = null;
        Refresh();
    }
}
