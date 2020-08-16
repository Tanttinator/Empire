using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HumanPlayer : PlayerController
{
    ClientController client;

    public Unit ActiveUnit => (activeUnits.Count > 0 ? activeUnits[0] : null);

    public event Action<Unit> onUnitSelected;
    public event Action<Unit> onUnitDeselected;

    List<Unit> activeUnits = new List<Unit>();

    public TileData[,] SeenTiles => player.seenTiles;

    public HumanPlayer(Player player) : base(player)
    {
        client = ClientController.instance;
    }

    /// <summary>
    /// Try to execute a command given by the player.
    /// </summary>
    /// <param name="command"></param>
    public void ExecuteCommand(PlayerCommand command)
    {
        command.Execute(this, ActiveUnit);
    }

    /// <summary>
    /// Removes current unit from the list of active units.
    /// </summary>
    public void NextUnit()
    {
        if (!active || activeUnits.Count == 0) return;

        onUnitDeselected?.Invoke(ActiveUnit);
        activeUnits.RemoveAt(0);

        if (activeUnits.Count == 0) EndTurn();
        else SelectUnit(ActiveUnit);
    }

    /// <summary>
    /// Set a unit as active.
    /// </summary>
    /// <param name="unit"></param>
    void SelectUnit(Unit unit)
    {
        if (unit == null) return;
        onUnitSelected?.Invoke(unit);
    }

    protected override void OnTurnStarted()
    {
        client.SetActivePlayer(this);

        activeUnits.AddRange(player.Units);
        foreach(Unit unit in player.Units)
        {
            unit.Refresh();
        }

        SelectUnit(ActiveUnit);
    }

    protected override void OnTurnEnded()
    {
        activeUnits.Clear();
    }

    public override void DoTurn()
    {
        if (ActiveUnit == null) return;
        if (ActiveUnit.DoTurn()) NextUnit();
    }

    public override string ToString()
    {
        return player.ToString();
    }

}
