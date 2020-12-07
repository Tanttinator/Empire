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
        public Dictionary<UnitType, int> progress = new Dictionary<UnitType, int>();

        public int efficiency = 50;

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
            tile.UpdateState(owner);
            owner.UpdatePlayer(owner.GetData());
        }

        /// <summary>
        /// Called when a unit tries to move onto this tile.
        /// </summary>
        /// <param name="unit"></param>
        public override bool Interact(Unit unit)
        {
            if (unit.owner != owner)
            {
                Battle(unit, this);
                return true;
            }
            return false;
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
                if (!progress.ContainsKey(production)) progress[production] = 0;
                progress[production] += efficiency;
                if (progress[production] >= production.productionCost)
                {
                    progress[production] = 0;
                    Unit.CreateUnit(production, tile, owner);
                }
            }
            tile?.UpdateState(owner);
        }

        public override void SetOwner(Player owner)
        {
            this.owner?.RemoveCity(this);
            base.SetOwner(owner);
            owner.AddCity(this);
            SetProduction(Unit.infantry);
        }

        protected override int CalculateStrength(Combatant enemy)
        {
            return efficiency / 50;
        }

        protected override void OnDefeat(Combatant enemy)
        {
            if(enemy is Unit unit)
            {
                if(unit.type.name == "Fighter")
                {
                    progress.Clear();
                }
                else if(unit.type.name == "Bomber")
                {
                    progress.Clear();
                    efficiency -= 5;
                }
                else if(unit.type.unitClass == UnitClass.SHIP)
                {

                }
                else
                {
                    SetOwner(enemy.owner);
                    efficiency -= 5;
                }
            }
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
                progress = progress
            };
        }
    }
}
