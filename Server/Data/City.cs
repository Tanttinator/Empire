using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

namespace Server
{
    public class City : Structure
    {

        public string name { get; protected set; }
        UnitType producedUnit = Unit.infantry;
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
        /// Called when the owner of this structure changes.
        /// </summary>
        /// <param name="oldOwner"></param>
        protected override void OnOwnerChanged(Player oldOwner)
        {
            if (oldOwner != null) oldOwner.onTurnStarted -= OnTurnStarted;
            this.owner.onTurnStarted += OnTurnStarted;
        }

        /// <summary>
        /// Called when the owners turn starts.
        /// </summary>
        void OnTurnStarted()
        {
            progress += production;
            if (progress >= producedUnit.productionCost)
            {
                progress = 0;
                Unit.CreateUnit(producedUnit, tile, owner);
            }
        }

        public override StructureData GetData()
        {
            return new CityData()
            {
                structure = "City",
                owner = owner.GetData(),
                name = name,
                producedUnit = producedUnit.name,
                production = production,
                remaining = Mathf.CeilToInt((producedUnit.productionCost - progress) * 1f / production)
            };
        }
    }
}
