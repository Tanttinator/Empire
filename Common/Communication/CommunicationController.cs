﻿using System.Collections;
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

        public static void StartTurn(Human human)
        {
            ClientController.StartTurn(human.ID, GameController.turn, human.currentState.Clone(), human.ActiveUnit.tile.coords);
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

        public static void UpdateTile(Player player, TileData tile)
        {
            //ClientController.UpdateTile(player.ID, tile);
        }

        public static void UpdatePlayer(Player player)
        {
            //ClientController.UpdatePlayer(player.ID, player.GetData());
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

        public static void ExecuteCommand(int ID, PlayerCommand command)
        {
            if (GameController.GetPlayer(ID) is Human human) human.ExecuteCommand(command);
        }

        public static void SetProduction(int city, UnitType unit)
        {
            City.cities[city].SetProduction(unit);
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
