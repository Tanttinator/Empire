using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    [System.Serializable]
    public class UnitType
    {
        public string name { get; protected set; }
        public UnitClass unitClass { get; protected set; }
        public int movement { get; protected set; }
        public int productionCost { get; protected set; }

        public UnitType(string name, UnitClass unitClass, int movement, int productionCost)
        {
            this.name = name;
            this.unitClass = unitClass;
            this.movement = movement;
            this.productionCost = productionCost;
        }
    }

    public enum UnitClass
    {
        INFANTRY,
        VEHICLE,
        SHIP,
        PLANE
    }
}