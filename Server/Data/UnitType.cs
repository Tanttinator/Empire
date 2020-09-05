using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitType
{
    public string name;
    public int movement;
    public int productionCost;

    public UnitType(string name, int movement, int productionCost)
    {
        this.name = name;
        this.movement = movement;
        this.productionCost = productionCost;
    }
}
