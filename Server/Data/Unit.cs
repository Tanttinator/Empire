﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;

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

        Tile target;
        Queue<Tile> currentPath;

        public static UnitType infantry = new UnitType("Infantry", UnitClass.INFANTRY, 1, 1, 500);
        public static UnitType tank = new UnitType("Tank", UnitClass.VEHICLE, 2, 2, 1000);
        public static UnitType fighter = new UnitType("Fighter", UnitClass.PLANE, 5, 1, 1000, 20);
        public static UnitType transport = new UnitType("Transport", UnitClass.SHIP, 2, 2, 1500);

        public static UnitType[] units = new UnitType[]
        {
            infantry,
            tank,
            fighter,
            transport
        };

        static int nextID = 0;

        public Unit(UnitType type, Tile tile, Player owner)
        {
            ID = nextID;
            nextID++;

            this.type = type;

            SetHealth(type.maxHealth);
            SetOwner(owner);
            SetTile(tile);
            Refuel();

            owner.AddUnit(this);
        }

        #region Movement

        public override void SetTile(Tile tile)
        {
            this.tile?.RemoveUnit(this);
            RemoveObserver();
            this.tile = tile;
            tile.PlaceUnit(this);
            AddObserver();
        }

        /// <summary>
        /// Try to move to the given target.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public bool Move(Tile tile)
        {
            if (!tile.CanEnter(this)) return false;
            SetTile(tile);
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
                    if(!nextTile.Interact(this)) target = null;
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
            tile.UpdateState();
            return unit;
        }
    }
}
