using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;

namespace Server
{
    public class Unit : Observer, ICombatant
    {
        public int ID { get; protected set; }
        public UnitType type { get; protected set; }
        public int moves { get; protected set; }
        public int health { get; protected set; }
        public bool sleeping { get; protected set; } = false;

        Player ICombatant.Owner => owner;
        Tile ICombatant.Tile => tile;

        Tile target;
        Queue<Tile> currentPath;

        public static UnitType infantry = new UnitType("Infantry", UnitClass.INFANTRY, 1, 1, 500);
        public static UnitType tank = new UnitType("Tank", UnitClass.VEHICLE, 2, 2, 1000);
        public static UnitType transport = new UnitType("Transport", UnitClass.SHIP, 2, 2, 1500);

        public static UnitType[] units = new UnitType[]
        {
            infantry,
            tank,
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

            owner.AddUnit(this);
        }

        #region Movement

        /// <summary>
        /// Place this unit on the given tile.
        /// </summary>
        /// <param name="tile"></param>
        protected override void OnTileChanged(Tile tile, Tile oldTile)
        {
            oldTile?.SetUnit(null);
            tile.SetUnit(this);
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

        #endregion

        #region Combat

        /// <summary>
        /// Attack the given unit.
        /// </summary>
        /// <param name="enemy"></param>
        public void Battle(ICombatant enemy)
        {
            bool attackerDead = false;
            bool defenderDead = false;

            Tile enemyTile = enemy.Tile;

            CommunicationController.SpawnExplosion(enemyTile, tile);
            CommunicationController.SpawnExplosion(tile, enemyTile);

            while (!attackerDead && !defenderDead)
            {
                if(UnityEngine.Random.Range(0, 2) == 0)
                {
                    CommunicationController.SpawnExplosion(tile, enemyTile);
                    attackerDead = TakeDamage();
                } 
                else
                {
                    CommunicationController.SpawnExplosion(enemyTile, tile);
                    defenderDead = enemy.TakeDamage();
                }

                if(attackerDead) Defeated(enemy);

                if(defenderDead)
                {
                    enemy.Defeated(this);
                    if (enemyTile.CanEnter(this))
                    {
                        SetTile(enemyTile);
                        enemyTile.UpdateState(enemy.Owner);
                    }
                    moves = 0;
                }
            }

            CommunicationController.UpdateState(0.3f);
        }

        /// <summary>
        /// Destroy this unit.
        /// </summary>
        void Destroy()
        {
            owner.RemoveUnit(this);
            tile.SetUnit(null);
            moves = 0;

            RemoveObserver();
        }

        public void SetHealth(int health)
        {
            this.health = Mathf.Clamp(health, 0, type.maxHealth);
        }

        public bool TakeDamage()
        {
            SetHealth(health - 1);
            return health == 0;
        }

        public void Defeated(ICombatant enemy)
        {
            Destroy();
        }

        #endregion

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

                if (!nextTile.Interact(this)) GeneratePath();
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
