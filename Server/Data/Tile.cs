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

        HashSet<Unit> seenBy = new HashSet<Unit>();
        public Player[] SeenBy
        {
            get
            {
                HashSet<Player> players = new HashSet<Player>();

                foreach (Unit unit in seenBy) players.Add(unit.owner);

                return players.ToArray();
            }
        }

        AStar.Vector2 INode.Position => coords;

        INode[] INode.Neighbors => World.GetNeighbors(this);

        public static event Action<Tile, Unit> onTileUnitSet;

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
                structure = (structure != null ? structure.ID : -1),
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
                player.UpdateTile(GetData(player));
            }
        }

        /// <summary>
        /// Called when the visibility of this tile changes for a player.
        /// </summary>
        /// <param name="player"></param>
        public void UpdateVisibility(Player player)
        {
            player.UpdateTile(GetData(player));
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
        public void AddObserver(Unit unit)
        {
            seenBy.Add(unit);
            UpdateVisibility(unit.owner);
            structure?.UpdateState(unit.owner);
        }

        /// <summary>
        /// The given unit no longer sees this tile.
        /// </summary>
        /// <param name="unit"></param>
        public void RemoveObserver(Unit unit)
        {
            seenBy.Remove(unit);
            UpdateVisibility(unit.owner);
        }

        #endregion

        #region Movement

        /// <summary>
        /// Called when the given unit tries to move onto this tile.
        /// </summary>
        /// <param name="unit"></param>
        public bool Interact(Unit unit)
        {
            if (structure != null) structure.Interact(unit);
            if (this.unit == null && CanEnter(unit))
            {
                return unit.Move(this);
            }

            if (this.unit != null && this.unit.owner != unit.owner)
            {
                unit.Battle(this.unit);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Can the given unit enter this tile?
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        bool CanEnter(Unit unit)
        {
            if (this.unit != null && this.unit.owner == unit.owner) return false;

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
            return 1;
        }

        #endregion

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
            structure.SetTile(this);

            UpdateState();
        }

        /// <summary>
        /// Place the unit on this tile.
        /// </summary>
        /// <param name="unit"></param>
        public void SetUnit(Unit unit)
        {
            this.unit = unit;
            onTileUnitSet?.Invoke(this, unit);
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
            TileData state = unit.owner.seenTiles[coords.x + coords.y * World.Width];
            if (state.discovered) return MovementCost(unit);
            return 1;
        }

        bool INode.CanEnter(object agent, INode from)
        {
            Unit unit = (Unit)agent;
            TileData state = unit.owner.seenTiles[coords.x + coords.y * World.Width];
            if (state.discovered) return CanEnter(unit);
            return true;
        }
    }
}
