using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{

    /// <summary>
    /// Move unit one step in the given direction.
    /// </summary>
    /// <param name="dir"></param>
    void Move(Direction dir)
    {
        if (LocalPlayer.ActiveUnit == null) return;

        Tile tile = World.GetTile(LocalPlayer.ActiveUnit.tile.coords.Neighbor(dir));
        if (tile != null) LocalPlayer.ActiveUnit.SetTile(tile);
    }

    /// <summary>
    /// Returns the tile which is under the mouse pointer currently.
    /// </summary>
    /// <returns></returns>
    Tile GetTileUnderMouse()
    {
        Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
        return WorldGraphics.GetTileAtPoint(point);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) Move(Direction.NORTH);
        if (Input.GetKeyDown(KeyCode.RightArrow)) Move(Direction.EAST);
        if (Input.GetKeyDown(KeyCode.DownArrow)) Move(Direction.SOUTH);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) Move(Direction.WEST);

        if (Input.GetKeyDown(KeyCode.Space)) LocalPlayer.activePlayer?.NextUnit();

        if(Input.GetMouseButtonDown(1))
        {
            Tile tile = GetTileUnderMouse();
            if (tile != null)
            {
                LocalPlayer.ActiveUnit?.SetTarget(tile);
                LocalPlayer.ActiveUnit?.MoveToTarget();
            }
        }
    }
}
