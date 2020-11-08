using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCamera;
using Common;
using Tanttinator.ModularUI;

namespace Client
{
    public class ClientController : MonoBehaviour
    {
        public static int activePlayer { get; protected set; } = -1;
        static int activeUnit = -1;
        public static UnitData ActiveUnit
        {
            get
            {
                return gameState.GetUnit(activeUnit);
            }
        }
        public static GameState gameState { get; protected set; }


        [SerializeField] new RTSCameraController2D camera = default;
        public static RTSCameraController2D CameraController => instance.camera;
        [SerializeField] LineRenderer movementIndicator = default;

        public static UnitType[] unitTypes;

        static InputState inputState = new DefaultState();

        static Coords hoverTile;
        bool[] isDragging;
        Vector3[] clickPos;
        bool pressed = false;
        
        static Queue<Sequence> sequenceQueue = new Queue<Sequence>();
        static Sequence currentSequence;
        float cooldown = 0f;

        static ClientController instance;

        public static void Initialize(int width, int height, PlayerData[] players, UnitType[] unitTypes)
        {
            World.InitTiles(width, height);
            ClientController.unitTypes = unitTypes;
        }

        public static void SetState(GameState state)
        {
            gameState = state;
            World.DrawState(state);
        }

        public static void UpdateState(int player, GameState state)
        {
            AddSequence(new StateUpdateSequence(player, state, 0.3f));
        }

        public static void StartTurn(int player, int turn, GameState state, Coords focusTile)
        {
            //Debug.Log("Start Turn, Player: " + player);
            AddSequence(new StartTurnSequence(player, turn, state, focusTile));
        }

        public static void TurnCompleted(int player)
        {
            //Debug.Log("Turn Completed, Player: " + player);
            AddSequence(new ControlSequence("Turn Completed", () =>
            {
                CommunicationController.EndTurn(player);
            }));
        }

        public static void SelectUnit(int unit)
        {
            //Debug.Log("Select Unit, Unit: " + unit);
            AddSequence(new ControlSequence("Select Unit", () =>
            {
                activeUnit = unit;
            }));
            AddSequence(new MoveCameraToUnitSequence(unit));
        }

        public static void DeselectUnit()
        {
            //Debug.Log("Deselect Unit");
            AddSequence(new ControlSequence("Deselect Unit", () =>
            {
                activeUnit = -1;
                ChangeState(new DefaultState());
            }));
        }

        public static void AddSequence(int player, Sequence sequence)
        {
            if (activePlayer == player) AddSequence(sequence);
        }

        public static void ChangeActivePlayer(int player)
        {
            activePlayer = player;
        }

        #region Input

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

        /// <summary>
        /// Handle all mouse interactions.
        /// </summary>
        /// <param name="button"></param>
        void HandleMouse(int button)
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

        #endregion

        #region Sequencer

        /// <summary>
        /// Add new sequence to be executed.
        /// </summary>
        /// <param name="sequence"></param>
        public static void AddSequence(Sequence sequence)
        {
            //Debug.Log("Added sequence: " + sequence);
            sequenceQueue.Enqueue(sequence);
        }

        #endregion

        private void Update()
        {
            if (currentSequence != null)
            {
                if (currentSequence.Update())
                {
                    currentSequence.End();
                    currentSequence = null;
                    cooldown = 0f;
                }
            }
            else if (sequenceQueue.Count > 0)
            {
                if (cooldown == 0f)
                {
                    currentSequence = sequenceQueue.Dequeue();
                    //Debug.Log("Started sequence: " + currentSequence);
                    currentSequence.Start();
                }
            }

            cooldown = Mathf.Max(0f, cooldown - Time.deltaTime); 
            
            inputState.Update();
            HandleMouse(0);
            HandleMouse(1);

            Coords newHoverTile = GetCoordsUnderMouse();
            if (World.ValidCoords(newHoverTile) && newHoverTile != hoverTile)
            {
                hoverTile = newHoverTile;
                inputState.HoverEnter(hoverTile);
            }
        }

        private void Awake()
        {
            instance = this;

            isDragging = new bool[] { false, false };
            clickPos = new Vector3[] { Vector3.zero, Vector3.zero };
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
                    TileInfoUI.Show(ClientController.gameState.GetTile(hoverTile));
                }
            }
        }
    }

    public class DefaultState : CameraMoveState
    {
        public override void Update()
        {
            if (ClientController.ActiveUnit != null) ClientController.ChangeState(new UnitSelectedState(ClientController.ActiveUnit));
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
            gfx = World.GetTileGraphics(unit.tile).Unit;
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

        public override void Click(int button, Coords hoverTile)
        {
            base.Click(button, hoverTile);
            if (button == 1)
            {
                if (hoverTile != null) CommunicationController.ExecuteCommand(ClientController.activePlayer, new CommandMove(hoverTile));
            }
        }

        public override void DragStart(int button)
        {
            base.DragStart(button);
            if (button == 1)
            {
                ClientController.ChangeState(new DragMoveState(unit.tile, gfx));
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
            ClientController.DrawMovementIndicator(pos, hoverTile);
        }

        public override void HoverEnter(Coords tile)
        {
            ClientController.DrawMovementIndicator(pos, tile);
        }

        public override void End()
        {
            ClientController.HideMovementIndicator();
        }

        public override void DragEnd(int button)
        {
            if (button == 1)
            {
                Coords tile = ClientController.GetCoordsUnderMouse();
                if (World.ValidCoords(tile)) CommunicationController.ExecuteCommand(ClientController.activePlayer, new CommandMove(tile));
                ClientController.ChangeState(new DefaultState());
            }
        }

        public override void Click(int button, Coords hoverTile)
        {

        }
    }
}
