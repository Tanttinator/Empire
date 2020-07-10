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
        Tile tile = World.GetTile(UnitController.unit.tile.coords.Neighbor(dir));
        if (tile != null) UnitController.unit.SetTile(tile);
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

        if(Input.GetMouseButtonDown(0))
        {
            Tile tile = GetTileUnderMouse();
            if (tile != null)
            {
                UnitController.unit.SetTarget(tile);
                UnitController.unit.MoveToTarget();
            }
        }
    }
}
