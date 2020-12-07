using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AStar;
using System.Linq;
using Common;

namespace Server
{
    /// <summary>
    /// Holds data for one tile in the map.
    /// </summary>
    public class Tile : INode
    {
        public Coords coords { get; protected set; }
        public bool land { get; protected set; }
        public Feature feature { get; protected set; }
        public Structure structure { get; protected set; }
        public Unit unit { get; protected set; }
        public Island island = null;

        public bool CanBuildStructure => land && feature == null && structure == null;

        public bool IsCoastal
        {
            get
            {
                if (!land) return false;

                foreach (Tile neighbor in World.GetNeighbors(this))
                {
                    if (!neighbor.land) return true;
                }

                return false;
            }
        }

        HashSet<Combatant> seenBy = new HashSet<Combatant>();
        public Player[] SeenBy
        {
            get
            {
                HashSet<Player> players = new HashSet<Player>();

                foreach (Combatant observer in seenBy) players.Add(observer.owner);

                return players.ToArray();
            }
        }

        AStar.Vector2 INode.Position => coords;

        INode[] INode.Neighbors => World.GetNeighbors(this);

        public Tile(Coords coords, bool land)
        {
            this.coords = coords;
            this.land = land;
        }

        #region Visibility

        /// <summary>
        /// Create tiledata object that represents this tile.
        /// </summary>
        /// <returns></returns>
        public TileData GetData(Player player)
        {
            return new TileData()
            {
                coords = coords,
                discovered = true,
                land = land,
                landConnections = new bool[]
                {
                    ConnectTexture(Direction.NORTH),
                    ConnectTexture(Direction.EAST),
                    ConnectTexture(Direction.SOUTH),
                    ConnectTexture(Direction.WEST)
                },
                feature = (feature != null ? feature.name : "empty"),
                featureConnections = new bool[]
                {
                    ConnectFeatureTexture(Direction.NORTH),
                    ConnectFeatureTexture(Direction.EAST),
                    ConnectFeatureTexture(Direction.SOUTH),
                    ConnectFeatureTexture(Direction.WEST)
                },
                unit = (unit != null ? unit.ID : -1),
                structure = (structure != null ? structure.GetData() : null),
                visible = SeenBy.Contains(player)
            };
        }

        /// <summary>
        /// Update the seen state of this tile to all players who currently have vision on it.
        /// </summary>
        public void UpdateState()
        {
            foreach (Player player in SeenBy)
            {
                UpdateState(player);
            }
        }

        /// <summary>
        /// Provide the player with the current state of this tile.
        /// </summary>
        /// <param name="player"></param>
        public void UpdateState(Player player)
        {
            player.UpdateTile(GetData(player));
            if (unit != null) player.UpdateUnit(unit.GetData());
        }

        /// <summary>
        /// Reveal this tile to a player.
        /// </summary>
        /// <param name="player"></param>
        public void Reveal(Player player)
        {
            player.UpdateTile(GetData(player));
        }

        /// <summary>
        /// Add a unit who can see this tile.
        /// </summary>
        /// <param name="unit"></param>
        public void AddObserver(Combatant observer)
        {
            seenBy.Add(observer);
            UpdateState(observer.owner);
        }

        /// <summary>
        /// The given unit no longer sees this tile.
        /// </summary>
        /// <param name="unit"></param>
        public void RemoveObserver(Combatant observer)
        {
            seenBy.Remove(observer);
            UpdateState(observer.owner);
        }

        #endregion

        #region Movement

        /// <summary>
        /// Called when the given unit tries to move onto this tile.
        /// </summary>
        /// <param name="unit"></param>
        public bool Interact(Unit unit)
        {
            if (structure != null && structure.Interact(unit)) return true;
            if (this.unit != null && this.unit.Interact(unit)) return true;
            return unit.Move(this);
        }

        /// <summary>
        /// Called when a unit moves to this tile.
        /// </summary>
        /// <param name="unit"></param>
        public void PlaceUnit(Unit unit)
        {
            this.unit = unit;

            UpdateState();
        }

        /// <summary>
        /// Called when a unit leaves this tile.
        /// </summary>
        /// <param name="unit"></param>
        public void RemoveUnit(Unit unit)
        {
            if (this.unit == unit) this.unit = null;

            UpdateState();
        }

        /// <summary>
        /// Can the given unit enter this tile?
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public bool CanEnter(Unit unit)
        {
            if (this.unit != null) return false;
            if (structure != null && structure.owner != unit.owner) return false;
            switch (unit.type.unitClass)
            {
                case UnitClass.INFANTRY: return land;
                case UnitClass.VEHICLE: return land && feature != Feature.mountains;
                case UnitClass.SHIP: return !land || structure is City;
                case UnitClass.PLANE: return true;
                default: return false;
            }
        }

        /// <summary>
        /// How many movement points does it cost for the given unit to enter this tile?
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public int MovementCost(Unit unit)
        {
            if (unit.type.unitClass == UnitClass.PLANE) return 1;
            return 1 + (feature != null? feature.movementCost : 0);
        }

        #endregion

        /// <summary>
        /// Is there something hostile to the given player on this tile.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool Hostile(Player player)
        {
            return (unit != null && unit.owner != player) || (structure != null && structure.owner != player);
        }

        /// <summary>
        /// Set the feature on this tile.
        /// </summary>
        /// <param name="feature"></param>
        public void SetFeature(Feature feature)
        {
            this.feature = feature;

            UpdateState();
        }

        /// <summary>
        /// Set the structure on this tile.
        /// </summary>
        /// <param name="structure"></param>
        public void SetStructure(Structure structure)
        {
            this.structure = structure;

            UpdateState();
        }

        /// <summary>
        /// Should our ground texture connect to the one in the given direction?
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        bool ConnectTexture(Direction dir)
        {
            return World.GetNeighbor(this, dir) == null || World.GetNeighbor(this, dir).land == land;
        }

        /// <summary>
        /// Should our feature texture connect to the one in the given direction?
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        bool ConnectFeatureTexture(Direction dir)
        {
            return feature != null && feature.ConnectTexture(World.GetNeighbor(this, dir));
        }

        float INode.EntryCost(object agent, INode from)
        {
            Unit unit = (Unit)agent;
            TileData state = unit.owner.currentState.GetTile(coords);
            if (state.discovered)
            {
                if (unit.moves == 1) return 1;
                return MovementCost(unit);
            }
            return 10;
        }

        bool INode.CanEnter(object agent, INode from)
        {
            Unit unit = (Unit)agent;
            TileData state = unit.owner.currentState.GetTile(coords);
            if (state.discovered) return CanEnter(unit);
            return true;
        }
    }
}
