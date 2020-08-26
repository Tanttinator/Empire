using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData
{
    public Ground ground;
    public bool[] groundConnections;
    public Feature feature;
    public bool[] featureConnections;
    public UnitData unit;
    public StructureData structure;
    public bool visible;
}
