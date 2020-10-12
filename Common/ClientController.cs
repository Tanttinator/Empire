using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCamera;
using Server;
using Client;

namespace Common
{
    public class ClientController : MonoBehaviour
    {
        static int activePlayer = -1;
        public static Coords? activeUnit { get; protected set; }

        [SerializeField] new RTSCameraController2D camera = default;
        public static RTSCameraController2D Camera => instance.camera;

        public static UnitType[] units;

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

        public static void Init(int width, int height, UnitType[] units)
        {
            Client.World.InitTiles(width, height);
            GameState.Init(width, height);
            ClientController.units = units;
        }

        public static void SelectUnit(Coords coords)
        {
            activeUnit = coords;
            //InputController.ChangeState(new UnitSelectedState(coords));
        }

        public static void DeselectUnit()
        {
            activeUnit = null;
            InputController.ChangeState(new DefaultState());
        }

        public static void ExecuteCommand(PlayerCommand command)
        {
            ((Human)GameController.GetPlayer(activePlayer)).ExecuteCommand(command);
        }

        public static void AddSequence(Sequence sequence)
        {
            Sequencer.AddSequence(sequence);
        }

        public static void EndTurn()
        {
            ((Human)GameController.GetPlayer(activePlayer)).EndTurn();
        }

        private void Awake()
        {
            instance = this;
        }
    }
}
