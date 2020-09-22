using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

namespace Client
{
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
                SpriteOrientationData groundSprite = SpriteRegistry.GetSprite((state.land? "Land" : "Water")).GetSprite(state.landConnections[0], state.landConnections[1], state.landConnections[2], state.landConnections[3]);
                groundGfx.sprite = groundSprite.sprite;
                groundGfx.transform.rotation = Quaternion.Euler(0f, 0f, groundSprite.rotation);
                if (state.feature == "empty") featureGfx.enabled = false;
                else
                {
                    featureGfx.enabled = true;
                    SpriteOrientationData featureSprite = SpriteRegistry.GetSprite(state.feature).GetSprite(state.featureConnections[0], state.featureConnections[1], state.featureConnections[2], state.featureConnections[3]);
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
}
