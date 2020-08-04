using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public Color color { get; protected set; }

    List<Unit> units = new List<Unit>();
    public Unit[] Units => units.ToArray();

    public Player(Color color)
    {
        this.color = color;
    }

    public void AddUnit(Unit unit)
    {
        units.Add(unit);
    }
}
