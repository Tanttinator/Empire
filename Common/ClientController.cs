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
        public static Coords? ActiveUnit
        {
            get
            {
                UnitData unit = GameState.GetUnit(activeUnit);
                return unit?.tile;
            }
        }

        [SerializeField] new RTSCameraController2D camera = default;
        public static RTSCameraController2D Camera => instance.camera;

        public static UnitType[] unitTypes;

        public static ClientController instance;

        /// <summary>
        /// Switch viewpoint to the given player.
        /// </summary>
        /// <param name="playerID"></param>
        public static void SetActivePlayer(int playerID, Coords focusTile)
        {
            if (activePlayer != playerID)
            {
                Camera.Translate(Client.World.GetTilePosition(focusTile) - Camera.transform.position);
            }

            activePlayer = playerID;
        }

        public static void Initialize(int width, int height, PlayerData[] players, UnitType[] unitTypes)
        {
            Client.World.InitTiles(width, height);
            GameState.Init(width, height, players);
            ClientController.unitTypes = unitTypes;
        }

        public static void SelectUnit(int unit)
        {
            activeUnit = unit;
            //InputController.ChangeState(new UnitSelectedState(coords));
        }

        public static void DeselectUnit()
        {
            activeUnit = -1;
            InputController.ChangeState(new DefaultState());
        }

        public static void ExecuteCommand(PlayerCommand command)
        {
            CommunicationController.ExecuteCommand(activePlayer, command);
        }

        public static void AddSequence(Sequence sequence)
        {
            Sequencer.AddSequence(sequence);
        }

        public static void EndTurn()
        {
            CommunicationController.EndTurn(activePlayer);
        }

        private void Awake()
        {
            instance = this;
        }
    }
}
