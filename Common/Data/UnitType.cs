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
        public int maxHealth { get; protected set; }
        public int productionCost { get; protected set; }
        public int maxFuel { get; protected set; }
        public int cargoSize { get; protected set; }
        public int cargoSpace { get; protected set; }
        public UnitType[] cargoTypes { get; protected set; }

        public UnitType(string name, UnitClass unitClass, int movement, int maxHealth, int productionCost, int maxFuel = -1, int cargoSize = 1, int cargoSpace = 0, params UnitType[] cargoTypes)
        {
            this.name = name;
            this.unitClass = unitClass;
            this.movement = movement;
            this.maxHealth = maxHealth;
            this.productionCost = productionCost;
            this.maxFuel = maxFuel;
            this.cargoSize = cargoSize;
            this.cargoSpace = cargoSpace;
            this.cargoTypes = cargoTypes;
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