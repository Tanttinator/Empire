using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

namespace Server
{
    public class City : Structure
    {

        public string name { get; protected set; }
        public UnitType production { get; protected set; } = Unit.infantry;
        public int efficiency = 50;
        int progress = 0;

        public static List<City> cities = new List<City>();

        public City() : base("City")
        {
            cities.Add(this);
            name = "City " + ID;
        }

        /// <summary>
        /// Set the unit which is produced in this city.
        /// </summary>
        /// <param name="unit"></param>
        public void SetProduction(UnitType unit)
        {
            production = unit;
            progress = 0;
            tile.UpdateState(owner);
            CommunicationController.UpdatePlayer(owner);
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
            if (production == null)
            {
                efficiency += 5;
            }
            else
            {
                progress += efficiency;
                if (progress >= production.productionCost)
                {
                    progress = 0;
                    Unit.CreateUnit(production, tile, owner);
                }
            }
            tile?.UpdateState(owner);
        }

        protected override void OnOwnerChanged(Player owner, Player oldOwner)
        {
            base.OnOwnerChanged(owner, oldOwner);
            oldOwner?.RemoveCity(this);
            owner.AddCity(this);
            SetProduction(Unit.infantry);
        }

        public override StructureData GetData()
        {
            return new CityData()
            {
                ID = ID,
                structure = "City",
                owner = owner.ID,
                name = name,
                production = production,
                efficiency = efficiency,
                remaining = (production != null? Mathf.CeilToInt((production.productionCost - progress) * 1f / efficiency) : 0)
            };
        }
    }
}
