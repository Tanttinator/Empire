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
                return CurrentState.GetUnit(activeUnit);
            }
        }
        static Dictionary<int, GameState> playerStates = new Dictionary<int, GameState>();
        public static GameState CurrentState => playerStates[activePlayer];


        [SerializeField] new RTSCameraController2D camera = default;
        public static RTSCameraController2D Camera => instance.camera;

        public static UnitType[] unitTypes;

        public static ClientController instance;

        public static void Initialize(int[] myPlayers, int width, int height, PlayerData[] players, UnitType[] unitTypes)
        {
            World.InitTiles(width, height);
            ClientController.unitTypes = unitTypes;
            foreach(int ID in myPlayers)
            {
                playerStates[ID] = new GameState(width, height, players);
            }
        }

        public static void UpdateTile(int player, TileData tile)
        {
            if (playerStates.ContainsKey(player)) playerStates[player].UpdateTile(tile);
        }

        public static void UpdatePlayer(int player, PlayerData data)
        {
            if (playerStates.ContainsKey(player)) playerStates[player].UpdatePlayer(data);
        }

        public static void Redraw(int player, float delay)
        {
            if (activePlayer == player) Sequencer.AddSequence(new RedrawSequence(player, delay));
        }

        public static void StartTurn(int player, Coords focusTile)
        {
            //Debug.Log("Start Turn, Player: " + player);
            if (playerStates.ContainsKey(player))
            {
                Sequencer.AddSequence(new ControlSequence(() =>
                {
                    if (activePlayer != player)
                    {
                        Camera.Translate(World.GetTilePosition(focusTile) - Camera.transform.position);
                    }

                    activePlayer = player;

                    World.DrawState(playerStates[player]);
                }));
            }
        }

        public static void TurnCompleted(int player)
        {
            //Debug.Log("Turn Completed, Player: " + player);
            if (activePlayer == player)
            {
                Sequencer.AddSequence(new ControlSequence(() =>
                {
                    CommunicationController.EndTurn(player);
                }));
            }
        }

        public static void SelectUnit(int unit)
        {
            //Debug.Log("Select Unit, Unit: " + unit);
            Sequencer.AddSequence(new ControlSequence(() =>
            {
                activeUnit = unit;
            }));
            Sequencer.AddSequence(new MoveCameraToUnitSequence());
        }

        public static void DeselectUnit()
        {
            //Debug.Log("Deselect Unit");
            Sequencer.AddSequence(new ControlSequence(() =>
            {
                activeUnit = -1;
                InputController.ChangeState(new DefaultState());
            }, 0.3f));
        }

        public static void ExecuteCommand(PlayerCommand command)
        {
            CommunicationController.ExecuteCommand(activePlayer, command);
        }

        public static GameState GetState(int player)
        {
            if (playerStates.ContainsKey(player)) return playerStates[player];
            return null;
        }

        private void Awake()
        {
            instance = this;
        }
    }
}
