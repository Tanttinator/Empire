using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField] UnitType[] units = default;
    public static UnitType[] Units => instance.units;

    static UnitController instance;

    /// <summary>
    /// Spawn a new instance of a unit.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="tile"></param>
    public static Unit SpawnUnit(UnitType type, Tile tile, Player owner)
    {
        return new Unit(type, tile, owner);
    }

    private void Awake()
    {
        instance = this;
    }
}
