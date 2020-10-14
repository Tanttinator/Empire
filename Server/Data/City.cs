using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

namespace Server
{
    public class City : Structure
    {

        public string name { get; protected set; }
        public UnitType producedUnit { get; protected set; } = Unit.infantry;
        public int production = 1;
        int progress = 0;

        public static List<City> cities = new List<City>();

        public City() : base("City")
        {
            cities.Add(this);
            name = "City";
        }

        /// <summary>
        /// Called when a unit tries to move onto this tile.
        /// </summary>
        /// <param name="unit"></param>
        public override void Interact(Unit unit)
        {
            SetOwner(unit.owner);
        }

        /// <summary>
        /// Advance the production in this city.
        /// </summary>
        public void Produce()
        {
            progress += production;
            if (progress >= producedUnit.productionCost)
            {
                progress = 0;
                Unit.CreateUnit(producedUnit, tile, owner);
            }
        }

        protected override void OnOwnerChanged(Player oldOwner)
        {
            oldOwner?.RemoveCity(this);
            owner.AddCity(this);
        }

        public override StructureData GetData()
        {
            return new CityData()
            {
                ID = ID,
                structure = "City",
                owner = owner.ID,
                name = name,
                producedUnit = producedUnit.name,
                production = production,
                remaining = Mathf.CeilToInt((producedUnit.productionCost - progress) * 1f / production)
            };
        }
    }
}
