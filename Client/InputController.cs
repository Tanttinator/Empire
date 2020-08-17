using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{

    static InputState currentState = new DefaultState();

    /// <summary>
    /// Change the current input state.
    /// </summary>
    /// <param name="state"></param>
    public static void ChangeState(InputState state)
    {
        currentState.End();
        currentState = state;
        currentState.Start();
    }

    /// <summary>
    /// Returns the coords of the tile which is under the mouse pointer currently.
    /// </summary>
    /// <returns></returns>
    public static Coords GetCoordsUnderMouse()
    {
        Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
        return WorldGraphics.GetTileAtPoint(point);
    }

    void OnAnimationStart()
    {
        ChangeState(new DefaultState());
    }

    void OnAnimationEnd()
    {
        if (ClientController.activeUnit != null) ChangeState(new UnitSelectedState(ClientController.activeUnit.Value));
    }

    void Update()
    {
        currentState.Update();
    }

    private void Awake()
    {
        Sequencer.onIdleEnd += OnAnimationStart;
        Sequencer.onIdleStart += OnAnimationEnd;
    }

    private void OnDisable()
    {
        Sequencer.onIdleEnd -= OnAnimationStart;
        Sequencer.onIdleStart -= OnAnimationEnd;
    }
}

public abstract class InputState
{
    public virtual void Start()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void End()
    {

    }
}

public class DefaultState : InputState
{

}

public class UnitSelectedState : InputState
{
    UnitGraphics unit;

    public UnitSelectedState(Coords unit)
    {
        this.unit = WorldGraphics.GetTileGraphics(unit).Unit;
    }

    public override void Start()
    {
        unit.SetIdle(true);
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) ClientController.activePlayer?.ExecuteCommand(new CommandMoveDir(Direction.NORTH));
        if (Input.GetKeyDown(KeyCode.RightArrow)) ClientController.activePlayer?.ExecuteCommand(new CommandMoveDir(Direction.EAST));
        if (Input.GetKeyDown(KeyCode.DownArrow)) ClientController.activePlayer?.ExecuteCommand(new CommandMoveDir(Direction.SOUTH));
        if (Input.GetKeyDown(KeyCode.LeftArrow)) ClientController.activePlayer?.ExecuteCommand(new CommandMoveDir(Direction.WEST));

        if (Input.GetKeyDown(KeyCode.Space)) ClientController.activePlayer?.ExecuteCommand(new CommandWait());

        if (Input.GetMouseButtonDown(1))
        {
            Coords coords = InputController.GetCoordsUnderMouse();
            if (coords != null)
            {
                ClientController.activePlayer?.ExecuteCommand(new CommandMove(coords));
            }
        }
    }

    public override void End()
    {
        unit.SetIdle(false);
    }
}
