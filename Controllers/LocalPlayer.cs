using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayer : PlayerController
{
    public static LocalPlayer activePlayer { get; protected set; }
    public static Unit ActiveUnit {
        get 
        {
            Unit unit = (activePlayer != null && activePlayer.activeUnits.Count > 0 ? activePlayer.activeUnits[0] : null);
            return unit; 
        }
    }

    List<Unit> activeUnits = new List<Unit>();

    public LocalPlayer(Player player) : base(player)
    {

    }

    /// <summary>
    /// Removes current unit from the list of active units.
    /// </summary>
    public void NextUnit()
    {
        if (!active) return;

        activeUnits.RemoveAt(0);

        if (activeUnits.Count == 0) EndTurn();
    }

    protected override void OnTurnStarted()
    {
        activePlayer = this;
        activeUnits.AddRange(player.Units);
        Debug.Log("Start Turn: " + this);
    }

    protected override void OnTurnEnded()
    {
        activePlayer = null;
        activeUnits.Clear();
    }

    public override string ToString()
    {
        return player.ToString();
    }

}
