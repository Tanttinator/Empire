using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Structure
{
    public StructureType type { get; protected set; }
    public Player owner { get; protected set; }
    public Tile tile { get; protected set; }

    public event Action onOwnerChanged;

    public Structure(StructureType type)
    {
        this.type = type;
        SetOwner(GameController.neutral);
    }

    public void SetTile(Tile tile)
    {
        this.tile = tile;
    }

    public void SetOwner(Player owner)
    {
        Player oldOwner = this.owner;
        this.owner = owner;
        OnOwnerChanged(oldOwner);
        onOwnerChanged?.Invoke();
    }

    protected virtual void OnOwnerChanged(Player oldOwner)
    {

    }

    public virtual void Interact(Unit unit)
    {

    }

    public StructureData GetData()
    {
        return new StructureData()
        {
            structure = type,
            color = owner.color
        };
    }

    /// <summary>
    /// Create a new structure.
    /// </summary>
    /// <param name="structure"></param>
    /// <param name="tile"></param>
    /// <param name="owner"></param>
    public static void CreateStructure(Structure structure, Tile tile, Player owner)
    {
        structure.SetOwner(owner);
        tile.SetStructure(structure);
    }
}

public enum StructureType
{
    CITY
}
