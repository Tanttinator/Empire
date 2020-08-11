using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayer : PlayerController
{
    public static LocalPlayer activePlayer { get; protected set; }
    public static Unit ActiveUnit => (activePlayer != null && activePlayer.activeUnits.Count > 0 ? activePlayer.activeUnits[0] : null);

    List<Unit> activeUnits = new List<Unit>();

    public LocalPlayer(Player player) : base(player)
    {

    }

    /// <summary>
    /// Removes current unit from the list of active units.
    /// </summary>
    public void NextUnit()
    {
        if (!active || activeUnits.Count == 0) return;

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
    }

    protected override void OnTurnStarted()
    {
        activePlayer = this;
        activeUnits.AddRange(player.Units);
        foreach(Unit unit in player.Units)
        {
            unit.Refresh();
        }
        SelectUnit(ActiveUnit);
        Debug.Log("Start Turn: " + this);
    }

    protected override void OnTurnEnded()
    {
        activePlayer = null;
        activeUnits.Clear();
    }

    public override void DoTurn()
    {
        if (ActiveUnit.DoTurn()) NextUnit();
    }

    public override string ToString()
    {
        return player.ToString();
    }

}
