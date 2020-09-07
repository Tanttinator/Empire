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
    public static Sprite GetUnitSprite(string unit)
    {
        foreach(UnitSpriteData data in instance.unitSprites)
        {
            if (data.unit == unit) return data.sprite;
        }
        Debug.LogError("No sprite registered for unit of type: " + unit);
        return null;
    }

    /// <summary>
    /// Creates an explosion effect on the given tile.
    /// </summary>
    /// <param name="position"></param>
    public static void SpawnExplosion(Tile tile)
    {
        Instantiate(instance.explosion, WorldGraphics.GetTilePosition(tile.coords), Quaternion.identity);
    }

    void OnUnitMoved(Unit unit, Tile from, Tile to)
    {
        Sequencer.AddSequence(new UnitMoveSequence(unit.GetData(), from.coords, to.coords, (TileData[,])unit.owner.SeenTiles));
    }

    void OnUnitDestroyed(Unit unit)
    {
        Sequencer.AddSequence(new UnitDieSequence(unit));
    }

    void OnUnitsBattled(Unit attacker, Unit defender, Unit[] hits)
    {
        Sequencer.AddSequence(new ExplosionSequence(defender.tile));
        Sequencer.AddSequence(new ExplosionSequence(attacker.tile));

        foreach(Unit unit in hits)
            Sequencer.AddSequence(new ExplosionSequence(unit.tile));
    }

    private void Awake()
    {
        instance = this;

        Unit.onUnitMoved += OnUnitMoved;
        Unit.onUnitDestroyed += OnUnitDestroyed;
        Unit.onUnitsBattled += OnUnitsBattled;
    }

    private void OnDisable()
    {
        Unit.onUnitMoved -= OnUnitMoved;
        Unit.onUnitDestroyed -= OnUnitDestroyed;
        Unit.onUnitsBattled -= OnUnitsBattled;
    }
}

[System.Serializable]
public struct UnitSpriteData
{
    public string unit;
    public Sprite sprite;
}

public class UnitMoveSequence : Sequence
{
    UnitData unit;
    Coords from;
    Coords to;
    TileData[,] vision;

    float progress = 0f;

    public UnitMoveSequence(UnitData unit, Coords from, Coords to, TileData[,] vision)
    {
        this.unit = unit;
        this.from = from;
        this.to = to;
        this.vision = vision;
    }

    public override void Start()
    {
        GameState.MoveUnit(unit, from, to);
        GameState.UpdateTiles(vision);
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
        GameState.RemoveUnit(unit.tile.coords);
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
