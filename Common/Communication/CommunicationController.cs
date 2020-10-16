using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;
using Server;
using System.Linq;

namespace Common
{
    public class CommunicationController : MonoBehaviour
    {

        #region Server -> Client

        public static void Initialize(int[] myPlayers, int width, int height, PlayerData[] players, UnitType[] unitTypes)
        {
            ClientController.Initialize(myPlayers, width, height, players, unitTypes);
        }

        public static void StartTurn(Human human)
        {
            ClientController.StartTurn(human.ID, human.ActiveUnit.tile.coords);
        }

        public static void TurnCompleted(Player player)
        {
            ClientController.TurnCompleted(player.ID);
        }

        public static void SelectUnit(Unit unit)
        {
            ClientController.SelectUnit(unit.ID);
        }

        public static void DeselectUnit()
        {
            ClientController.DeselectUnit();
        }

        public static void UpdateState()
        {
            foreach (Player player in GameController.players) ClientController.UpdateState(player.ID, player.currentState);
        }

        public static void UpdateTile(Tile tile)
        {
            foreach (Player player in tile.SeenBy) ClientController.UpdateState(player.ID, player.currentState);
        }

        public static void MoveUnit(Tile from, Tile to)
        {
            foreach(Player player in SeenBy(from, to))
            {
                ClientController.UpdateState(player.ID, player.currentState, 0.3f);
            }
        }

        public static void KillUnit(Unit unit, Tile tile)
        {
            foreach(Player player in SeenBy(tile))
            {
                ClientController.UpdateState(player.ID, player.currentState);
            }
            ClientController.UpdateState(unit.owner.ID, unit.owner.currentState);
        }

        #endregion

        #region Client -> Server

        public static void EndTurn(int ID)
        {
            GameController.GetPlayer(ID).EndTurn();
        }

        public static void ExecuteCommand(int ID, PlayerCommand command)
        {
            if (GameController.GetPlayer(ID) is Human human) human.ExecuteCommand(command);
        }

        #endregion

        static Player[] SeenBy(params Tile[] tiles)
        {
            HashSet<Player> seenBy = new HashSet<Player>();

            foreach(Tile tile in tiles)
            {
                foreach (Player player in tile.SeenBy) seenBy.Add(player);
            }

            return seenBy.ToArray();
        }
    }
}
