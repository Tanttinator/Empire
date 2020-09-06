using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData
{
    public Coords coords;
    public bool land;
    public bool[] landConnections;
    public string feature;
    public bool[] featureConnections;
    public UnitData unit;
    public StructureData structure;
    public bool visible;

    public override string ToString()
    {
        return coords + "\n"
            + (land? "Land" : "Water") + "\n" 
            + (feature != "empty"? feature + "\n" : "") 
            + (unit != null? unit.unit + "\n" : "")
            + (structure != null? structure.structure + "\n" : "");
    }
}
