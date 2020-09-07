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

    public UnitGraphics Unit => unitGfx;

    /// <summary>
    /// Refresh graphics of this tile.
    /// </summary>
    public void Refresh(TileData state)
    {
        if (state == null)
        {
            unexplored.enabled = true;
        }
        else
        {
            SpriteOrientationData groundSprite = WorldGraphics.GetGroundSprite(state);
            groundGfx.sprite = groundSprite.sprite;
            groundGfx.transform.rotation = Quaternion.Euler(0f, 0f, groundSprite.rotation);
            if (state.feature == "empty") featureGfx.enabled = false;
            else
            {
                featureGfx.enabled = true;
                SpriteOrientationData featureSprite = WorldGraphics.GetFeatureSprite(state);
                featureGfx.sprite = featureSprite.sprite;
                featureGfx.transform.rotation = Quaternion.Euler(0f, 0f, featureSprite.rotation);
            }
            unitGfx.SetUnit(state.unit);
            structureGfx.SetStructure(state.structure);
            fogOfWar.enabled = !state.visible;
            unexplored.enabled = false;
        }
    }
}
