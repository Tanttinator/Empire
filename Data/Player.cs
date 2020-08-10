using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public string name { get; protected set; }
    public Color color { get; protected set; }

    List<Unit> units = new List<Unit>();
    public Unit[] Units => units.ToArray();

    public Player(string name, Color color)
    {
        this.name = name;
        this.color = color;
    }

    public void AddUnit(Unit unit)
    {
        units.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        units.Remove(unit);
    }

    public override string ToString()
    {
        return name;
    }
}
