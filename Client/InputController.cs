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
        ClientController.activePlayer?.ExecuteCommand(new CommandMoveDir(dir));
    }

    /// <summary>
    /// Returns the coords of the tile which is under the mouse pointer currently.
    /// </summary>
    /// <returns></returns>
    Coords GetCoordsUnderMouse()
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

        if (Input.GetKeyDown(KeyCode.Space)) ClientController.activePlayer?.ExecuteCommand(new CommandWait());

        if(Input.GetMouseButtonDown(1))
        {
            Coords coords = GetCoordsUnderMouse();
            if (coords != null)
            {
                ClientController.activePlayer?.ExecuteCommand(new CommandMove(coords));
            }
        }
    }
}
