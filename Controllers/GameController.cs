using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    static PlayerController[] players;
    static int activePlayer = 0;

    static int turn = 1;

    /// <summary>
    /// Set the next player on the list as active.
    /// </summary>
    public static void NextPlayer()
    {
        activePlayer++;

        if(activePlayer >= players.Length)
        {
            turn++;
            activePlayer = 0;

            Debug.Log("Turn " + turn);
        }

        players[activePlayer].StartTurn();
    }

    static GameController instance;

    private void Start()
    {
        instance = this;
        players = new PlayerController[]
        {
            new LocalPlayer(new Player("Player 1", Color.red)),
            new LocalPlayer(new Player("Player 2", Color.blue))
        };

        World.GenerateWorld();
        WorldGraphics.InitTiles();
        UnitController.SpawnUnit(UnitController.Units[0], World.GetTile(0, 0), players[0].player);
        UnitController.SpawnUnit(UnitController.Units[0], World.GetTile(World.Width - 1, 0), players[1].player);
        UnitController.SpawnUnit(UnitController.Units[0], World.GetTile(0, World.Height - 1), players[0].player);
        UnitController.SpawnUnit(UnitController.Units[0], World.GetTile(World.Width - 1, World.Height - 1), players[1].player);

        players[activePlayer].StartTurn();
    }
}
