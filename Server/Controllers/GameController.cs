using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

namespace Server
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] bool revealMap = false;

        public static Player[] players { get; protected set; }
        static int activePlayer = 0;
        static Player ActivePlayer => players[activePlayer];

        static int turn = 1;

        public static Player neutral { get; protected set; }

        /// <summary>
        /// Set the next player on the list as active.
        /// </summary>
        public static void NextPlayer()
        {
            activePlayer++;

            if (activePlayer >= players.Length)
            {
                turn++;
                activePlayer = 0;
            }

            ActivePlayer.StartTurn();
        }

        public static Player GetPlayer(int ID)
        {
            return players[ID];
        }

        private void Start()
        {
            players = new Player[]
            {
                new Human("Player 1", Color.red),
                new Human("Player 2", Color.blue)
            };

            neutral = new Player("Neutral", Color.white);

            PlayerData[] allPlayers = new PlayerData[players.Length + 1];
            for (int i = 0; i < players.Length; i++) allPlayers[i] = players[i].GetData();
            allPlayers[players.Length] = neutral.GetData();

            foreach (Player player in players) player.Initialize(allPlayers);
            neutral.Initialize(allPlayers);

            CommunicationController.Initialize(new int[] { 0, 1 }, World.Width, World.Height, allPlayers, Unit.units);

            World.GenerateWorld(players);

            foreach (Player player in players)
            {
                if (revealMap) player.RevealMap();
            }

            ActivePlayer.StartTurn();
        }

        private void Update()
        {
            ActivePlayer.DoTurn();
        }
    }
}
