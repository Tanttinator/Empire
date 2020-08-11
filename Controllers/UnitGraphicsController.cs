﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGraphicsController : MonoBehaviour
{
    [SerializeField] UnitSpriteData[] unitSprites = default;

    static UnitGraphicsController instance;

    /// <summary>
    /// Play unit idle animation.
    /// </summary>
    public static void Idle()
    {

    }

    /// <summary>
    /// Find a sprite matching the given unit type in the registry.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public static Sprite GetUnitSprite(UnitType unit)
    {
        foreach(UnitSpriteData data in instance.unitSprites)
        {
            if (data.unit == unit) return data.sprite;
        }
        Debug.LogError("No sprite registered for unit of type: " + unit.name);
        return null;
    }

    /// <summary>
    /// Called when a unit of a tile changes.
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="unit"></param>
    void OnUnitMoved(Unit unit, Tile from, Tile to)
    {
        Sequencer.AddSequence(new UnitMoveSequence(unit, from, to));
    }

    /// <summary>
    /// Called when unit is destroyed.
    /// </summary>
    /// <param name="unit"></param>
    void OnUnitDestroyed(Unit unit)
    {
        Sequencer.AddSequence(new UnitDieSequence(unit));
    }

    /// <summary>
    /// Called when a unit is created.
    /// </summary>
    /// <param name="unit"></param>
    void OnUnitCreated(Unit unit)
    {
        WorldGraphics.GetTileGraphics(unit.tile.coords).SetUnit(unit);
    }

    private void Awake()
    {
        instance = this;

        Unit.onUnitMoved += OnUnitMoved;
        Unit.onUnitDestroyed += OnUnitDestroyed;
        Unit.onUnitCreated += OnUnitCreated;
    }

    private void OnDisable()
    {
        Unit.onUnitMoved -= OnUnitMoved;
        Unit.onUnitDestroyed -= OnUnitDestroyed;
        Unit.onUnitCreated -= OnUnitCreated;
    }
}

[System.Serializable]
public struct UnitSpriteData
{
    public UnitType unit;
    public Sprite sprite;
}

public class UnitMoveSequence : Sequence
{
    Unit unit;
    Tile from;
    Tile to;

    float progress = 0f;

    public UnitMoveSequence(Unit unit, Tile from, Tile to)
    {
        this.unit = unit;
        this.from = from;
        this.to = to;
    }

    public override void Start()
    {
        if(from != null) WorldGraphics.GetTileGraphics(from.coords).SetUnit(null);
        WorldGraphics.GetTileGraphics(to.coords).SetUnit(unit);
    }

    public override bool Update()
    {
        progress += Time.deltaTime;
        return progress >= 0.3f;
    }
}

public class UnitDieSequence : Sequence
{
    Unit unit;

    public UnitDieSequence(Unit unit)
    {
        this.unit = unit;
    }

    public override void Start()
    {
        WorldGraphics.GetTileGraphics(unit.tile.coords).SetUnit(null);
    }
}
