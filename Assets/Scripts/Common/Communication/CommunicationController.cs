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

        public static void Initialize(int width, int height, UnitType[] unitTypes)
        {
            ClientController.Initialize(width, height, unitTypes);
        }

        public static void StartTurn(Human human, Unit selectedUnit)
        {
            ClientController.StartTurn(human.ID, GameController.turn, human.currentState.Clone(), selectedUnit.tile.coords);
        }

        public static void TurnCompleted(Human human)
        {
            ClientController.TurnCompleted(human.ID);
        }

        public static void SelectUnit(Unit unit)
        {
            ClientController.SelectUnit(unit.ID);
        }

        public static void DeselectUnit()
        {
            ClientController.DeselectUnit();
        }

        public static void UpdateState(float delay)
        {
            foreach (Player player in GameController.players) ClientController.UpdateState(player.ID, player.currentState.Clone());
        }

        public static void SpawnExplosion(Tile tile, Tile other)
        {
            foreach (Player player in SeenBy(tile, other)) ClientController.AddSequence(player.ID, new ExplosionSequence(tile.coords));
        }

        #endregion

        #region Client -> Server

        public static void EndTurn(int ID)
        {
            GameController.GetPlayer(ID).EndTurn();
        }

        public static void ExecuteCommand(int ID, UnitCommand command)
        {
            if (GameController.GetPlayer(ID) is Human human) human.ExecuteCommand(command);
        }

        public static void SetProduction(int city, UnitType unit)
        {
            City.cities[city].SetProduction(unit);
        }

        public static void SetActiveUnit(int player, int unit)
        {
            if (GameController.GetPlayer(player) is Human human) human.SelectUnit(Unit.GetUnit(unit));
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
