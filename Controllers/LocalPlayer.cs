using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayer : PlayerController
{
    public static LocalPlayer activePlayer { get; protected set; }
    public static Unit ActiveUnit => activePlayer?.activeUnit;

    Unit activeUnit;

    public LocalPlayer(Player player) : base(player)
    {

    }

    protected override void OnTurnStarted()
    {
        activePlayer = this;
        activeUnit = player.Units[0];
    }

    protected override void OnTurnEnded()
    {
        activePlayer = null;
        activeUnit = null;
    }

}
