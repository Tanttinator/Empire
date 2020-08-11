using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerController
{

    public Player player { get; protected set; }

    protected bool active;

    public PlayerController(Player player)
    {
        this.player = player;
    }

    /// <summary>
    /// Tells GameController to cycle to the next player.
    /// </summary>
    public void EndTurn()
    {
        if (!active) return;

        active = false;
        OnTurnEnded();
        GameController.NextPlayer();
    }

    /// <summary>
    /// Callback for when this players turn has ended.
    /// </summary>
    protected virtual void OnTurnEnded()
    {

    }

    /// <summary>
    /// Called when this player becomes active.
    /// </summary>
    public void StartTurn()
    {
        if (active) return;

        active = true;
        OnTurnStarted();
    }

    /// <summary>
    /// Callback for when this players turn has started.
    /// </summary>
    protected virtual void OnTurnStarted()
    {

    }

    /// <summary>
    /// Called every frame while this player is active.
    /// </summary>
    public virtual void DoTurn()
    {

    }

}
