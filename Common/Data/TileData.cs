using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData
{
    public bool land;
    public bool[] landConnections;
    public string feature;
    public bool[] featureConnections;
    public UnitData unit;
    public StructureData structure;
    public bool visible;
}
