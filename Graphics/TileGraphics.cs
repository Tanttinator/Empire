using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible for graphical representation of a single tile.
/// </summary>
public class TileGraphics : MonoBehaviour
{
    Tile tile;

    [SerializeField] SpriteRenderer ground = default;

    /// <summary>
    /// Set the target tile.
    /// </summary>
    /// <param name="tile"></param>
    public void SetTile(Tile tile)
    {
        this.tile = tile;
    }
}
