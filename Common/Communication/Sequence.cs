using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;

namespace Common
{
    public class Sequence
    {
        float progress;

        public Sequence(float cooldown)
        {
            this.progress = cooldown;
        }

        public Sequence()
        {
            this.progress = 0f;
        }

        public virtual void Start()
        {

        }

        public virtual bool Update()
        {
            progress -= Time.deltaTime;
            return progress <= 0f;
        }

        public virtual void End()
        {

        }
    }

    public class UpdateTilesSequence : Sequence
    {
        TileData[] tiles;

        public UpdateTilesSequence(TileData[] tiles, float cooldown) : base(cooldown)
        {
            this.tiles = tiles;
        }

        public override void Start()
        {
            GameState.UpdateTiles(tiles);
        }
    }

    public class StartTurnSequence : Sequence
    {
        int player;
        TileData[] seenTiles;
        Coords focusTile;

        public StartTurnSequence(int player, TileData[] seenTiles, Coords focusTile) : base(0.3f)
        {
            this.player = player;
            this.seenTiles = seenTiles;
            this.focusTile = focusTile;
        }

        public override void Start()
        {
            ClientController.SetActivePlayer(player, focusTile);
            GameState.UpdateTiles(seenTiles);
        }
    }

    public class EndTurnSequence : Sequence
    {
        public override void Start()
        {
            ClientController.EndTurn();
        }
    }

    #region Units

    public class UpdateUnitSequence : Sequence
    {
        UnitData unit;

        public UpdateUnitSequence(UnitData unit)
        {
            this.unit = unit;
        }

        public override void Start()
        {
            GameState.UpdateUnit(unit);
        }
    }

    public class SelectUnitSequence : Sequence
    {
        int unit;
        float progress = 0f;

        public SelectUnitSequence(int unit)
        {
            this.unit = unit;
        }

        public override void Start()
        {
            ClientController.SelectUnit(unit);
            ClientController.Camera.MoveTowards(World.GetTilePosition(GameState.GetUnit(unit).tile));
        }

        public override bool Update()
        {
            if (!ClientController.Camera.isMovingToTarget)
            {
                progress += Time.deltaTime;
                return progress >= 0.3f;
            }
            return false;
        }
    }

    public class DeselectUnitSequence : Sequence
    {
        public DeselectUnitSequence() : base(0.3f)
        {

        }

        public override void Start()
        {
            ClientController.DeselectUnit();
        }
    }

    public class UnitMoveSequence : UpdateTilesSequence
    {
        public UnitMoveSequence(TileData[] tiles) : base(tiles, 0.3f)
        {

        }
    }

    public class UnitDieSequence : UpdateTilesSequence
    {
        public UnitDieSequence(TileData[] tiles) : base(tiles, 0.3f)
        {

        }
    }

    #endregion

    #region Structures

    public class UpdateStructureSequence : Sequence
    {
        StructureData structure;

        public UpdateStructureSequence(StructureData structure)
        {
            this.structure = structure;
        }

        public override void Start()
        {
            GameState.UpdateStructure(structure);
        }
    }

    #endregion

    public class ExplosionSequence : Sequence
    {
        Coords tile;

        public ExplosionSequence(Coords tile) : base(0.8f)
        {
            this.tile = tile;
        }

        public override void Start()
        {
            World.SpawnExplosion(tile);
        }
    }
}
