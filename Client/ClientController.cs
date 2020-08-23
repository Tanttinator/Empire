using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCamera;

public class ClientController : MonoBehaviour
{
    public static HumanPlayer activePlayer { get; protected set; }
    public static Coords? activeUnit { get; protected set; }

    [SerializeField] new RTSCameraController2D camera;
    public static RTSCameraController2D Camera => instance.camera;

    public static ClientController instance;

    /// <summary>
    /// Switch viewpoint to the given player.
    /// </summary>
    /// <param name="player"></param>
    public static void SetActivePlayer(HumanPlayer player)
    {
        activePlayer = player;
    }

    public static void Init(int width, int height)
    {
        WorldGraphics.InitTiles(width, height);
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

    private void Awake()
    {
        instance = this;
    }
}
