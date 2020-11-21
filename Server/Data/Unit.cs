using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
using System.Linq;

namespace Server
{
    public class Unit : Combatant
    {
        public int ID { get; protected set; }
        public UnitType type { get; protected set; }
        public int moves { get; protected set; }
        public int health { get; protected set; }
        public bool sleeping { get; protected set; } = false;
        public int fuel { get; protected set; }

        int RemainingCargoSpace
        {
            get
            {
                int space = 0;
                foreach (Unit unit in cargo) space += unit.type.cargoSize;
                return type.cargoSpace - space;
            }
        }
        List<Unit> cargo = new List<Unit>();
        Unit transportedIn = null;

        Tile target;
        Queue<Tile> currentPath;

        public static UnitType infantry = new UnitType("Infantry", UnitClass.INFANTRY, 1, 1, 400);
        public static UnitType tank = new UnitType("Tank", UnitClass.VEHICLE, 2, 2, 800, -1, 2);
        public static UnitType fighter = new UnitType("Fighter", UnitClass.PLANE, 5, 1, 800, 20);
        public static UnitType transport = new UnitType("Transport", UnitClass.SHIP, 2, 2, 1500, -1, 1, 6, infantry, tank);

        public static UnitType[] unitTypes = new UnitType[]
        {
            infantry,
            tank,
            fighter,
            transport
        };

        static List<Unit> units = new List<Unit>();

        public Unit(UnitType type, Tile tile, Player owner)
        {
            ID = units.Count;

            this.type = type;

            SetHealth(type.maxHealth);
            SetOwner(owner);
            SetTile(tile);
            tile.PlaceUnit(this);
            Refuel();

            owner.AddUnit(this);

            units.Add(this);
        }

        #region Movement

        public override void SetTile(Tile tile)
        {
            base.SetTile(tile);
            foreach (Unit unit in cargo) unit.SetTile(tile);
            UpdateState();
        }

        /// <summary>
        /// Moves the unit onto the tile.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public bool Move(Tile tile)
        {
            if (!tile.CanEnter(this)) return false;

            this.tile.RemoveUnit(this);
            if (transportedIn != null) transportedIn.RemoveCargo(this);
            if (sleeping) sleeping = false;

            SetTile(tile);
            tile.PlaceUnit(this);

            moves -= tile.MovementCost(this);

            CommunicationController.UpdateState(0.3f);

            if (fuel != -1)
            {
                fuel--;
                if (fuel == 0)
                {
                    CommunicationController.SpawnExplosion(tile, tile);
                    Destroy();
                    CommunicationController.UpdateState(0.3f);
                }
            }

            return true;
        }

        /// <summary>
        /// Moves the unit into the cargo space of another unit.
        /// </summary>
        /// <param name="unit"></param>
        public void MoveToCargo(Unit unit)
        {
            tile.RemoveUnit(this);
            if (transportedIn != null) transportedIn.RemoveCargo(this);
            if (sleeping) sleeping = false;

            unit.AddCargo(this);
            SetTile(unit.tile);
            moves = 0;

            CommunicationController.UpdateState(0.3f);
        }

        /// <summary>
        /// Set the target tile for this unit to move to.
        /// </summary>
        /// <param name="tile"></param>
        public void SetTarget(Tile tile)
        {
            target = tile;
        }

        /// <summary>
        /// Generate new path to the target.
        /// </summary>
        void GeneratePath()
        {
            currentPath = World.GetPath(this, target);
            if (currentPath != null)
                currentPath.Dequeue();
        }

        public void Refuel()
        {
            fuel = type.maxFuel;
        }

        #endregion

        #region Combat

        /// <summary>
        /// Destroy this unit.
        /// </summary>
        void Destroy()
        {
            owner.RemoveUnit(this);
            tile.RemoveUnit(this);
            moves = 0;

            foreach (Player player in tile.SeenBy) player.DestroyUnit(ID);

            foreach (Unit unit in cargo) unit.Destroy();

            RemoveObserver();
        }

        public void SetHealth(int health)
        {
            this.health = Mathf.Clamp(health, 0, type.maxHealth);
        }

        protected override bool TakeDamage()
        {
            SetHealth(health - 1);
            return health == 0;
        }

        protected override void OnVictory(Combatant enemy)
        {
            Move(enemy.tile);
            moves = 0;
        }

        protected override void OnDefeat(Combatant enemy)
        {
            Destroy();
        }

        #endregion

        #region Cargo

        /// <summary>
        /// Add a unit to this units cargo space.
        /// </summary>
        /// <param name="unit"></param>
        void AddCargo(Unit unit)
        {
            cargo.Add(unit);
            unit.Refuel();
        }

        /// <summary>
        /// Remove a unit from this units cargo space.
        /// </summary>
        /// <param name="unit"></param>
        void RemoveCargo(Unit unit)
        {
            cargo.Remove(unit);
        }

        /// <summary>
        /// Can we add the given unit to our cargo space?
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        bool CanEnterCargo(Unit unit)
        {
            return type.cargoTypes.Contains(unit.type) && RemainingCargoSpace >= unit.type.cargoSize;
        }
        #endregion

        /// <summary>
        /// Called when another unit tries to interact with this unit.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public bool Interact(Unit unit)
        {
            if (unit.owner != owner)
            {
                Battle(unit, this);
                return true;
            }

            if (CanEnterCargo(unit))
            {
                unit.MoveToCargo(this);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tell this unit to execute the next queued action if one exists.
        /// </summary>
        public bool DoTurn()
        {
            if (moves <= 0) return true;

            if (target != null && target != tile)
            {
                if (currentPath == null || currentPath.Count == 0) GeneratePath();

                if (currentPath == null)
                {
                    SetTarget(null);
                    return false;
                }

                Tile nextTile = currentPath.Dequeue();

                if (nextTile == target)
                {
                    nextTile.Interact(this);
                    target = null;
                }
                else if (nextTile.Hostile(owner)) target = null;
                else if (!Move(nextTile)) GeneratePath();
            }

            return false;
        }

        /// <summary>
        /// Called on the start of the owners turn.
        /// </summary>
        public void Refresh()
        {
            moves = type.movement;
        }

        /// <summary>
        /// Toggle unit sleeping.
        /// </summary>
        /// <param name="sleeping"></param>
        public void SetSleeping(bool sleeping)
        {
            this.sleeping = sleeping;
            moves = 0;
            UpdateState();
            CommunicationController.UpdateState(0f);
        }

        /// <summary>
        /// Update the state of this unit to all players who see it.
        /// </summary>
        void UpdateState()
        {
            foreach(Player player in tile.SeenBy) player.UpdateUnit(GetData());
        }

        public UnitData GetData()
        {
            return new UnitData()
            {
                ID = ID,
                unitType = type.name,
                owner = owner.ID,
                tile = tile.coords,
                sleeping = sleeping
            };
        }

        public override string ToString()
        {
            return owner + " " + type.name;
        }

        public static Unit CreateUnit(UnitType type, Tile tile, Player owner)
        {
            Unit unit = new Unit(type, tile, owner);
            return unit;
        }

        public static Unit GetUnit(int ID)
        {
            return units[ID];
        }
    }
}
