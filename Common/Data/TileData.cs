using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    [System.Serializable]
    public class TileData
    {
        public Coords coords;
        public bool discovered;
        public bool land;
        public bool[] landConnections;
        public string feature;
        public bool[] featureConnections;
        public UnitData unit;
        public StructureData structure;
        public bool visible;

        public override string ToString()
        {
            if (discovered)
                return "Tile: " + (land ? "Land" : "Sea") + "\n"
                     + (feature != "empty" ? "Feature: " + feature + "\n" : "");
            else
                return "Undiscovered";
        }
    }
}
