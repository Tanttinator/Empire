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

        public static void Initialize(int width, int height, PlayerData[] players, UnitType[] unitTypes)
        {
            ClientController.Initialize(width, height, players, unitTypes);
        }

        public static void AddSequence(Sequence sequence)
        {
            ClientController.AddSequence(sequence);
        }

        public static void MoveUnit(Unit unit, Tile from, Tile to)
        {
            foreach(Player player in SeenBy(from, to))
            {
                if (ClientController.activePlayer == player.ID)
                {
                    ClientController.AddSequence(new UnitMoveSequence(player.seenTiles, player.SeenStructures));
                    ClientController.AddSequence(new UpdateUnitSequence(unit.GetData()));
                }
            }
        }

        public static void KillUnit(Unit unit, Tile tile)
        {
            foreach(Player player in SeenBy(tile))
            {
                if (ClientController.activePlayer == player.ID) ClientController.AddSequence(new UnitDieSequence(player.seenTiles, player.SeenStructures));
            }
            ClientController.AddSequence(new UnitDieSequence(unit.owner.seenTiles, unit.owner.SeenStructures));
        }

        public static void CreateUnit(Unit unit)
        {
            ClientController.AddSequence(new UpdateUnitSequence(unit.GetData()));
        }

        public static void CreateStructure(Structure structure)
        {
            ClientController.AddSequence(new UpdateStructureSequence(structure.GetData()));
        }

        public static void UpdateStructure(Structure structure)
        {
            ClientController.AddSequence(new UpdateStructureSequence(structure.GetData()));
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
