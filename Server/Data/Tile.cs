using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AStar;
using System.Linq;

/// <summary>
/// Holds data for one tile in the map.
/// </summary>
public class Tile : INode
{
    public Coords coords { get; protected set; }
    public Ground ground { get; protected set; }
    public Feature feature { get; protected set; }
    public Structure structure { get; protected set; }
    public Unit unit { get; protected set; }
    public Island island = null;

    public bool IsCoastal
    {
        get
        {
            foreach(Tile neighbor in World.GetNeighbors(this))
            {
                if (neighbor.ground == World.Water) return true;
            }
            return false;
        }
    }

    HashSet<Unit> seenBy = new HashSet<Unit>();
    Player[] SeenBy
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

    public Tile(Coords coords, Ground ground)
    {
        this.coords = coords;
        this.ground = ground;
    }

    /// <summary>
    /// Create tiledata object that represents this tile.
    /// </summary>
    /// <returns></returns>
    public TileData GetData(Player player, TileData oldData, bool forceVisible = false)
    {
        if (CanSee(player) || forceVisible)
        {
            return new TileData()
            {
                ground = ground,
                groundConnections = new bool[]
                {
                    ConnectGroundTexture(Direction.NORTH),
                    ConnectGroundTexture(Direction.EAST),
                    ConnectGroundTexture(Direction.SOUTH),
                    ConnectGroundTexture(Direction.WEST)
                },
                feature = feature,
                featureConnections = new bool[]
                {
                    ConnectFeatureTexture(Direction.NORTH),
                    ConnectFeatureTexture(Direction.EAST),
                    ConnectFeatureTexture(Direction.SOUTH),
                    ConnectFeatureTexture(Direction.WEST)
                },
                unit = (unit != null ? unit.GetData() : null),
                structure = (structure != null ? structure.GetData() : null),
                visible = true
            };
        } else
        {
            if(oldData != null)
                oldData.visible = false;
            return oldData;
        }
    }

    /// <summary>
    /// Should our ground texture connect to the one in the given direction?
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    bool ConnectGroundTexture(Direction dir)
    {
        return World.GetNeighbor(this, dir) == null || World.GetNeighbor(this, dir).ground == ground;
    }

    /// <summary>
    /// Should our feature texture connect to the one in the given direction?
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    bool ConnectFeatureTexture(Direction dir)
    {
        return World.GetNeighbor(this, dir) == null || World.GetNeighbor(this, dir).feature == feature;
    }

    /// <summary>
    /// Refresh the seen state of this tile to all players who currently have vision on it.
    /// </summary>
    public void Refresh()
    {
        foreach(Player player in GameController.Players)
        {
            player.RefreshTile(this);
        }
    }

    /// <summary>
    /// Set the feature on this tile.
    /// </summary>
    /// <param name="feature"></param>
    public void SetFeature(Feature feature)
    {
        this.feature = feature;

        Refresh();
    }

    /// <summary>
    /// Set the structure on this tile.
    /// </summary>
    /// <param name="structure"></param>
    public void SetStructure(Structure structure)
    {
        if (this.structure != null) this.structure.onOwnerChanged -= Refresh;

        this.structure = structure;
        structure.SetTile(this);
        structure.onOwnerChanged += Refresh;

        Refresh();
    }

    /// <summary>
    /// Place the unit on this tile.
    /// </summary>
    /// <param name="unit"></param>
    public void SetUnit(Unit unit)
    {
        this.unit = unit;
        onTileUnitSet?.Invoke(this, unit);
        Refresh();
    }

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

        if (this.unit.owner != unit.owner)
        {
            unit.Battle(this.unit);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Add a unit who can see this tile.
    /// </summary>
    /// <param name="unit"></param>
    public void AddObserver(Unit unit)
    {
        seenBy.Add(unit);
        Refresh();
    }

    /// <summary>
    /// The given unit no longer sees this tile.
    /// </summary>
    /// <param name="unit"></param>
    public void RemoveObserver(Unit unit)
    {
        seenBy.Remove(unit);
        Refresh();
    }

    /// <summary>
    /// Does any unit of the given player have sight onto this tile.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool CanSee(Player player)
    {
        return SeenBy.Contains(player);
    }

    /// <summary>
    /// Can the given unit enter this tile?
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    bool CanEnter(Unit unit)
    {
        return this.unit == null || this.unit.owner != unit.owner;
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

    float INode.EntryCost(object agent, INode from)
    {
        return MovementCost(agent as Unit);
    }

    bool INode.CanEnter(object agent, INode from)
    {
        return CanEnter(agent as Unit);
    }
}
