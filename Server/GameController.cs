using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    static PlayerController[] players;
    static int activePlayer = 0;
    static PlayerController ActivePlayer => players[activePlayer];
    public static Player[] Players
    {
        get
        {
            if (GameController.players == null) return new Player[] { };
            Player[] players = new Player[GameController.players.Length];
            for(int i = 0; i < GameController.players.Length; i++)
            {
                players[i] = GameController.players[i].player;
            }
            return players;
        }
    }

    static int turn = 1;

    public static Player neutral { get; protected set; } = new Player("Neutral", Color.white);

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

        ActivePlayer.StartTurn();
    }

    static GameController instance;

    private void Start()
    {
        instance = this;

        World.GenerateWorld();
        ClientController.Init(World.Width, World.Height);

        players = new PlayerController[]
        {
            new HumanPlayer(new Player("Player 1", Color.red)),
            new HumanPlayer(new Player("Player 2", Color.blue))
        };

        foreach (PlayerController controller in players)
        {
            controller.player.InitVision();
        }

        UnitController.SpawnUnit(UnitController.Units[0], World.GetTile(0, 0), players[0].player);
        UnitController.SpawnUnit(UnitController.Units[0], World.GetTile(World.Width - 1, 0), players[1].player);
        UnitController.SpawnUnit(UnitController.Units[0], World.GetTile(0, World.Height - 1), players[0].player);
        UnitController.SpawnUnit(UnitController.Units[0], World.GetTile(World.Width - 1, World.Height - 1), players[1].player);

        ActivePlayer.StartTurn();
    }

    private void Update()
    {
        ActivePlayer.DoTurn();
    }
}
