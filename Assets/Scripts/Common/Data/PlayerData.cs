using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    [System.Serializable]
    public class PlayerData
    {
        public int ID;
        public string name;
        public Color color;
        public Dictionary<UnitType, int> production;
    }
}
