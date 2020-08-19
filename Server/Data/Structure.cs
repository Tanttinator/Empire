using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure
{
    public StructureType type { get; protected set; }
    public Player owner { get; protected set; }
    public Tile tile { get; protected set; }

    public Structure(StructureType type, Tile tile)
    {
        this.type = type;
        this.tile = tile;
        SetOwner(GameController.neutral);
    }

    public void SetOwner(Player owner)
    {
        this.owner = owner;
    }

    public StructureData GetData()
    {
        return new StructureData()
        {
            structure = type,
            color = owner.color
        };
    }
}
