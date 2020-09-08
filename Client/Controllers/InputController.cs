using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

namespace Client
{
    public class InputController : MonoBehaviour
    {

        static InputState currentState = new DefaultState();

        bool[] isDragging;
        Vector3[] clickPos;

        static Coords hoverTile;

        [SerializeField] LineRenderer movementIndicator = default;

        static InputController instance;

        /// <summary>
        /// Change the current input state.
        /// </summary>
        /// <param name="state"></param>
        public static void ChangeState(InputState state)
        {
            currentState.End();
            currentState = state;
            currentState.Start(hoverTile);
        }

        /// <summary>
        /// Cancel current state and figure out the next one.
        /// </summary>
        public static void CancelState()
        {
            if (ClientController.activeUnit != null) ChangeState(new UnitSelectedState(ClientController.activeUnit.Value));
            else ChangeState(new DefaultState());
        }

        /// <summary>
        /// Returns the coords of the tile which is under the mouse pointer currently.
        /// </summary>
        /// <returns></returns>
        public static Coords GetCoordsUnderMouse()
        {
            Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
            return World.GetTileAtPoint(point);
        }

        /// <summary>
        /// Draw a from start to end.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public static void DrawMovementIndicator(Coords start, Coords end)
        {
            instance.movementIndicator.SetPositions(new Vector3[] { (Vector2)start, (Vector2)end });
            instance.movementIndicator.enabled = true;
        }

        /// <summary>
        /// Hide the movement indicator line.
        /// </summary>
        public static void HideMovementIndicator()
        {
            instance.movementIndicator.enabled = false;
        }

        void OnAnimationStart()
        {
            ChangeState(new DefaultState());
        }

        void OnAnimationEnd()
        {
            CancelState();
        }

        /// <summary>
        /// Handle all mouse interactions.
        /// </summary>
        /// <param name="button"></param>
        void HandleMouse(int button)
        {
            if (Input.GetMouseButtonDown(button))
            {
                currentState.MouseDown(button);
                clickPos[button] = Input.mousePosition;
            }

            if (Input.GetMouseButton(button))
            {
                if (isDragging[button]) currentState.Drag(button);
                else if (Vector3.Distance(clickPos[button], Input.mousePosition) > 5f)
                {
                    isDragging[button] = true;
                    currentState.DragStart(button);
                }
                else currentState.MouseHold(button);
            }

            if (Input.GetMouseButtonUp(button))
            {
                if (isDragging[button])
                {
                    currentState.DragEnd(button);
                    isDragging[button] = false;
                }
                else currentState.Click(button, hoverTile);
            }
        }

        void Update()
        {
            Coords newHoverTile = GetCoordsUnderMouse();
            if (World.ValidCoords(newHoverTile) && newHoverTile != hoverTile)
            {
                hoverTile = newHoverTile;
                currentState.HoverEnter(hoverTile);
            }

            currentState.Update();
            HandleMouse(0);
            HandleMouse(1);
        }

        private void Awake()
        {
            instance = this;

            Sequencer.onIdleEnd += OnAnimationStart;
            Sequencer.onIdleStart += OnAnimationEnd;

            isDragging = new bool[] { false, false };
            clickPos = new Vector3[] { Vector3.zero, Vector3.zero };
        }

        private void OnDisable()
        {
            Sequencer.onIdleEnd -= OnAnimationStart;
            Sequencer.onIdleStart -= OnAnimationEnd;
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

    public class DefaultState : InputState
    {
        public override void Update()
        {
            if (Input.GetKey(KeyCode.UpArrow)) ClientController.Camera.Move(new Vector2(0, 1));
            if (Input.GetKey(KeyCode.RightArrow)) ClientController.Camera.Move(new Vector2(1, 0));
            if (Input.GetKey(KeyCode.DownArrow)) ClientController.Camera.Move(new Vector2(0, -1));
            if (Input.GetKey(KeyCode.LeftArrow)) ClientController.Camera.Move(new Vector2(-1, 0));

            ClientController.Camera.Zoom(Input.GetAxis("Mouse ScrollWheel"));
        }

        public override void DragStart(int button)
        {
            if (button == 0) ClientController.Camera.Drag(true);
        }

        public override void Drag(int button)
        {
            if (button == 0) ClientController.Camera.Drag();
        }

        public override void Click(int button, Coords hoverTile)
        {
            if (button == 0)
            {
                if (hoverTile != null)
                {
                    TileInfoUI.Show(GameState.GetTile(hoverTile));
                }
            }
        }
    }

    public class UnitSelectedState : DefaultState
    {
        UnitGraphics unit;
        Coords pos;

        public UnitSelectedState(Coords unit)
        {
            pos = unit;
            this.unit = World.GetTileGraphics(unit).Unit;
        }

        public override void Start(Coords hoverTile)
        {
            unit.SetIdle(true);
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Space)) ClientController.ExecuteCommand(new CommandWait());
        }

        public override void End()
        {
            unit.SetIdle(false);
        }

        public override void Click(int button, Coords hoverTile)
        {
            base.Click(button, hoverTile);
            if (button == 1)
            {
                if (hoverTile != null) ClientController.ExecuteCommand(new CommandMove(hoverTile));
            }
        }

        public override void DragStart(int button)
        {
            base.DragStart(button);
            if (button == 1)
            {
                InputController.ChangeState(new DragMoveState(pos, unit));
            }
        }
    }

    public class DragMoveState : DefaultState
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
            unit.SetIdle(false);
            InputController.DrawMovementIndicator(pos, hoverTile);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void HoverEnter(Coords tile)
        {
            InputController.DrawMovementIndicator(pos, tile);
        }

        public override void End()
        {
            InputController.HideMovementIndicator();
        }

        public override void DragEnd(int button)
        {
            if (button == 1)
            {
                Coords tile = InputController.GetCoordsUnderMouse();
                if (World.ValidCoords(tile)) ClientController.ExecuteCommand(new CommandMove(tile));
                InputController.CancelState();
            }
        }

        public override void Click(int button, Coords hoverTile)
        {

        }
    }
}