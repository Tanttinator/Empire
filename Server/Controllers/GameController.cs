using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] bool revealMap = false;

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

        ClientController.Init(World.Width, World.Height);

        Player[] players = new Player[]
        {
            new Player("Player 1", Color.red),
            new Player("Player 2", Color.blue)
        };

        GameController.players = new PlayerController[players.Length];
        for(int i = 0; i < players.Length; i++)
        {
            PlayerController player = GameController.players[i] = new HumanPlayer(players[i]);
            player.player.InitVision();
        }

        World.GenerateWorld(players);

        foreach (PlayerController player in GameController.players)
        {
            if (revealMap) player.player.RevealMap();
        }

        ActivePlayer.StartTurn();
    }

    private void Update()
    {
        ActivePlayer.DoTurn();
    }
}
