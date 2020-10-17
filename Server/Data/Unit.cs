using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;

namespace Server
{
    public class Unit
    {
        public int ID { get; protected set; }
        public UnitType type { get; protected set; }
        public Tile tile { get; protected set; }
        public Player owner { get; protected set; }
        public int moves { get; protected set; }
        public bool sleeping { get; protected set; } = false;

        Tile target;
        Queue<Tile> currentPath;

        Tile[] visibleTiles;

        public static UnitType infantry = new UnitType("Infantry", UnitClass.INFANTRY, 1, 500);
        public static UnitType transport = new UnitType("Transport", UnitClass.SHIP, 3, 1500);

        public static UnitType[] units = new UnitType[]
        {
            infantry,
            transport
        };

        static int nextID = 0;

        public Unit(UnitType type, Tile tile, Player owner)
        {
            ID = nextID;
            nextID++;

            this.type = type;
            this.owner = owner;

            SetTile(tile);

            owner.AddUnit(this);
        }

        #region Movement

        /// <summary>
        /// Place this unit on the given tile.
        /// </summary>
        /// <param name="tile"></param>
        public void SetTile(Tile tile)
        {
            this.tile?.SetUnit(null);

            this.tile = tile;
            tile.SetUnit(this);

            RefreshVision();
        }

        /// <summary>
        /// Try to move to the given target.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public bool Move(Tile tile, bool forced = false)
        {
            if (moves <= 0 && !forced) return false;

            Tile oldTile = this.tile;
            SetTile(tile);
            moves -= tile.MovementCost(this);
            CommunicationController.Redraw(0.3f);
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
        public void Battle(Unit enemy)
        {
            if (enemy.owner == owner) return;

            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                //TODO: Play battle animation.
                //onUnitsBattled?.Invoke(this, enemy, new Unit[] { this });
                Destroy();
            }
            else
            {
                Tile tile = enemy.tile;
                //onUnitsBattled?.Invoke(this, enemy, new Unit[] { enemy });
                enemy.Destroy();
                Move(tile, true);
                moves = 0;
            }
        }

        /// <summary>
        /// Destroy this unit.
        /// </summary>
        void Destroy()
        {
            owner.RemoveUnit(this);
            tile.SetUnit(null);
            moves = 0;

            foreach (Tile tile in visibleTiles) tile.RemoveObserver(this);

            CommunicationController.Redraw(0.3f);
        }

        #endregion

        #region Vision

        /// <summary>
        /// Update the tiles which this unit can see.
        /// </summary>
        void RefreshVision()
        {
            if (visibleTiles != null)
            {
                foreach (Tile tile in visibleTiles) tile.RemoveObserver(this);
            }

            visibleTiles = GetTilesInVision();
            foreach (Tile tile in visibleTiles) tile.AddObserver(this);
        }

        /// <summary>
        /// Returns all tiles within this units visibility.
        /// </summary>
        /// <returns></returns>
        Tile[] GetTilesInVision()
        {
            List<Tile> tiles = new List<Tile>();

            tiles.Add(tile);
            tiles.AddRange(World.GetNeighbors(tile));

            return tiles.ToArray();
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
