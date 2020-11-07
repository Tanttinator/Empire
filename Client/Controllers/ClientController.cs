using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCamera;
using Common;

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
                return currentState.GetUnit(activeUnit);
            }
        }
        public static GameState currentState { get; protected set; }


        [SerializeField] new RTSCameraController2D camera = default;
        public static RTSCameraController2D Camera => instance.camera;

        public static UnitType[] unitTypes;

        public static ClientController instance;

        public static void Initialize(int width, int height, PlayerData[] players, UnitType[] unitTypes)
        {
            World.InitTiles(width, height);
            ClientController.unitTypes = unitTypes;
        }

        public static void SetState(GameState state)
        {
            currentState = state;
            World.DrawState(state);
        }

        public static void UpdateState(int player, GameState state)
        {
            Sequencer.AddSequence(new StateUpdateSequence(player, state, 0.3f));
        }

        public static void StartTurn(int player, int turn, GameState state, Coords focusTile)
        {
            //Debug.Log("Start Turn, Player: " + player);
            Sequencer.AddSequence(new StartTurnSequence(player, turn, state, focusTile));
        }

        public static void TurnCompleted(int player)
        {
            //Debug.Log("Turn Completed, Player: " + player);
            Sequencer.AddSequence(new ControlSequence("Turn Completed", () =>
            {
                CommunicationController.EndTurn(player);
            }));
        }

        public static void SelectUnit(int unit)
        {
            //Debug.Log("Select Unit, Unit: " + unit);
            Sequencer.AddSequence(new ControlSequence("Select Unit", () =>
            {
                activeUnit = unit;
            }));
            Sequencer.AddSequence(new MoveCameraToUnitSequence(unit));
        }

        public static void DeselectUnit()
        {
            //Debug.Log("Deselect Unit");
            Sequencer.AddSequence(new ControlSequence("Deselect Unit", () =>
            {
                activeUnit = -1;
                InputController.ChangeState(new DefaultState());
            }));
        }

        public static void AddSequence(int player, Sequence sequence)
        {
            if (activePlayer == player) Sequencer.AddSequence(sequence);
        }

        public static void ChangeActivePlayer(int player)
        {
            activePlayer = player;
        }

        private void Awake()
        {
            instance = this;
        }
    }
}
