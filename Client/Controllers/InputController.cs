using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tanttinator.ModularUI;
using Common;

namespace Client
{
    public static class InputController
    {
        static InputState inputState = new DefaultState();

        static Coords hoverTile;
        static bool[] isDragging = new bool[] { false, false };
        static Vector3[] clickPos = new Vector3[] { Vector3.zero, Vector3.zero };
        static bool pressed = false;

        /// <summary>
        /// Change the current input state.
        /// </summary>
        /// <param name="state"></param>
        public static void ChangeState(InputState state)
        {
            inputState.End();
            inputState = state;
            inputState.Start(hoverTile);
        }

        /// <summary>
        /// Returns the coords of the tile which is under the mouse pointer currently.
        /// </summary>
        /// <returns></returns>
        public static Coords GetCoordsUnderMouse()
        {
            Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
            return WorldGraphics.GetTileAtPoint(point);
        }

        /// <summary>
        /// Handle all mouse interactions.
        /// </summary>
        /// <param name="button"></param>
        static void HandleMouse(int button)
        {
            if (Input.GetMouseButtonDown(button) && Hoverable.MouseOver == null)
            {
                pressed = true;
                inputState.MouseDown(button);
                clickPos[button] = Input.mousePosition;
            }

            if (Input.GetMouseButton(button) && pressed)
            {
                if (isDragging[button]) inputState.Drag(button);
                else if (Vector3.Distance(clickPos[button], Input.mousePosition) > 5f)
                {
                    isDragging[button] = true;
                    inputState.DragStart(button);
                }
                else inputState.MouseHold(button);
            }

            if (Input.GetMouseButtonUp(button) && pressed)
            {
                if (isDragging[button])
                {
                    inputState.DragEnd(button);
                    isDragging[button] = false;
                }
                else inputState.Click(button, hoverTile);
                pressed = false;
            }
        }

        public static void Update()
        {
            inputState.Update();
            HandleMouse(0);
            HandleMouse(1);

            Coords newHoverTile = GetCoordsUnderMouse();
            if (WorldGraphics.ValidCoords(newHoverTile) && newHoverTile != hoverTile)
            {
                hoverTile = newHoverTile;
                inputState.HoverEnter(hoverTile);
            }
        }
    }

    public abstract class InputState
    {
        public virtual void Start(Coords hoverTile)
        {

        }

        public virtual void Update()
        {

        }

        public virtual void End()
        {

        }

        public virtual void HoverEnter(Coords tile)
        {

        }

        public virtual void MouseDown(int button)
        {

        }

        public virtual void MouseHold(int button)
        {

        }

        public virtual void Click(int button, Coords hoverTile)
        {

        }

        public virtual void DragStart(int button)
        {

        }

        public virtual void Drag(int button)
        {

        }

        public virtual void DragEnd(int button)
        {

        }
    }

    public class CameraMoveState : InputState
    {
        public override void Update()
        {
            if (Input.GetKey(KeyCode.UpArrow)) ClientController.CameraController.Move(new Vector2(0, 1));
            if (Input.GetKey(KeyCode.RightArrow)) ClientController.CameraController.Move(new Vector2(1, 0));
            if (Input.GetKey(KeyCode.DownArrow)) ClientController.CameraController.Move(new Vector2(0, -1));
            if (Input.GetKey(KeyCode.LeftArrow)) ClientController.CameraController.Move(new Vector2(-1, 0));

            ClientController.CameraController.Zoom(Input.GetAxis("Mouse ScrollWheel"));
        }

        public override void DragStart(int button)
        {
            if (button == 0) ClientController.CameraController.Drag(true);
        }

        public override void Drag(int button)
        {
            if (button == 0) ClientController.CameraController.Drag();
        }

        public override void Click(int button, Coords hoverTile)
        {
            if (button == 0)
            {
                if (hoverTile != null)
                {
                    TileData tile = ClientController.gameState.GetTile(hoverTile);
                    if(tile.unit != -1 && tile.unit != ClientController.ActiveUnit.ID) CommunicationController.SetActiveUnit(ClientController.activePlayer, tile.unit);
                    else TileInfoUI.Show(tile);
                }
            }
        }
    }

    public class DefaultState : CameraMoveState
    {
        public override void Update()
        {
            if (ClientController.ActiveUnit != null) InputController.ChangeState(new UnitSelectedState(ClientController.ActiveUnit));
            base.Update();
        }
    }

    public class UnitSelectedState : CameraMoveState
    {
        UnitData unit;
        UnitGraphics gfx;

        public UnitSelectedState(UnitData unit)
        {
            this.unit = unit;
            gfx = WorldGraphics.GetTileGraphics(unit.tile).Unit;
        }

        public override void Start(Coords hoverTile)
        {
            gfx.SetActiveUnit(unit);
            gfx.StartIdle();
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Space)) CommunicationController.ExecuteCommand(ClientController.activePlayer, new CommandWait());
            if (Input.GetKeyDown(KeyCode.S)) CommunicationController.ExecuteCommand(ClientController.activePlayer, new CommandSleep());
        }

        public override void End()
        {
            gfx.StopIdle();
        }

        public override void DragStart(int button)
        {
            base.DragStart(button);
            if (button == 1)
            {
                InputController.ChangeState(new DragMoveState(unit.tile, gfx));
            }
        }
    }

    public class DragMoveState : CameraMoveState
    {
        Coords pos;
        UnitGraphics unit;

        public DragMoveState(Coords pos, UnitGraphics unit)
        {
            this.pos = pos;
            this.unit = unit;
        }

        public override void Start(Coords hoverTile)
        {
            unit.StopIdle();
            UnitMovementIndicator.Show(pos, hoverTile);
        }

        public override void HoverEnter(Coords tile)
        {
            UnitMovementIndicator.Show(pos, tile);
        }

        public override void End()
        {
            UnitMovementIndicator.Hide();
        }

        public override void DragEnd(int button)
        {
            if (button == 1)
            {
                Coords tile = InputController.GetCoordsUnderMouse();
                if (WorldGraphics.ValidCoords(tile)) CommunicationController.ExecuteCommand(ClientController.activePlayer, new CommandMove(tile));
                InputController.ChangeState(new DefaultState());
            }
        }

        public override void Click(int button, Coords hoverTile)
        {

        }
    }
}
