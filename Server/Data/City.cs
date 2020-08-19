using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : Structure
{
    public City() : base(StructureType.CITY)
    {

    }

    public override void Interact(Unit unit)
    {
        SetOwner(unit.owner);
    }
}
