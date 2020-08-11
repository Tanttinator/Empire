using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGraphicsController : MonoBehaviour
{
    [SerializeField] UnitSpriteData[] unitSprites = default;
    [SerializeField] GameObject explosion = default;

    static UnitGraphicsController instance;

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
    /// Find the graphics representing the given unit.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public static UnitGraphics GetUnitGraphics(Unit unit)
    {
        return WorldGraphics.GetTileGraphics(unit.tile.coords).UnitGraphics;
    }

    /// <summary>
    /// Creates an explosion effect on the given tile.
    /// </summary>
    /// <param name="position"></param>
    public static void SpawnExplosion(Tile tile)
    {
        Instantiate(instance.explosion, WorldGraphics.GetTilePosition(tile.coords), Quaternion.identity);
    }

    /// <summary>
    /// Start / Stop unit idle animation.
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="idle"></param>
    void SetIdle(Unit unit, bool idle)
    {
        if (unit == null) return;
        GetUnitGraphics(unit).SetIdle(idle);
    }

    void OnUnitMoved(Unit unit, Tile from, Tile to)
    {
        Sequencer.AddSequence(new UnitMoveSequence(unit, from, to));
    }

    void OnUnitDestroyed(Unit unit)
    {
        Sequencer.AddSequence(new UnitDieSequence(unit));
    }

    void OnUnitCreated(Unit unit)
    {
        WorldGraphics.GetTileGraphics(unit.tile.coords).SetUnit(unit);
    }

    void OnUnitsBattled(Unit attacker, Unit defender, Unit[] hits)
    {
        Sequencer.AddSequence(new ExplosionSequence(defender.tile));
        Sequencer.AddSequence(new ExplosionSequence(attacker.tile));

        foreach(Unit unit in hits)
            Sequencer.AddSequence(new ExplosionSequence(unit.tile));
    }

    void OnUnitDeselected(Unit unit)
    {
        SetIdle(unit, false);
    }

    void OnUnitSelected(Unit unit)
    {
        SetIdle(unit, Sequencer.idle);
    }

    void OnIdleStarted()
    {
        SetIdle(LocalPlayer.ActiveUnit, true);
    }

    void OnIdleEnded()
    {
        SetIdle(LocalPlayer.ActiveUnit, false);
    }

    private void Awake()
    {
        instance = this;

        Unit.onUnitMoved += OnUnitMoved;
        Unit.onUnitDestroyed += OnUnitDestroyed;
        Unit.onUnitCreated += OnUnitCreated;
        Unit.onUnitsBattled += OnUnitsBattled;

        LocalPlayer.onUnitDeselected += OnUnitDeselected;
        LocalPlayer.onUnitSelected += OnUnitSelected;

        Sequencer.onIdleStart += OnIdleStarted;
        Sequencer.onIdleEnd += OnIdleEnded;
    }

    private void OnDisable()
    {
        Unit.onUnitMoved -= OnUnitMoved;
        Unit.onUnitDestroyed -= OnUnitDestroyed;
        Unit.onUnitCreated -= OnUnitCreated;
        Unit.onUnitsBattled -= OnUnitsBattled;

        LocalPlayer.onUnitDeselected -= OnUnitDeselected;
        LocalPlayer.onUnitSelected -= OnUnitSelected;

        Sequencer.onIdleStart -= OnIdleStarted;
        Sequencer.onIdleEnd -= OnIdleEnded;
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

public class ExplosionSequence : Sequence
{
    Tile tile;

    float progress = 0f;

    public ExplosionSequence(Tile tile)
    {
        this.tile = tile;
    }

    public override void Start()
    {
        UnitGraphicsController.SpawnExplosion(tile);
    }

    public override bool Update()
    {
        progress += Time.deltaTime;
        return progress >= 0.8f;
    }
}
