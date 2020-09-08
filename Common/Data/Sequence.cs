using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;

namespace Common
{
    public class Sequence
    {
        public virtual void Start()
        {

        }

        public virtual bool Update()
        {
            return true;
        }

        public virtual void End()
        {

        }
    }

    public class StartTurnSequence : Sequence
    {
        int playerID;
        TileData[,] seenTiles;
        Coords focusTile;

        public StartTurnSequence(int playerID, TileData[,] seenTiles, Coords focusTile)
        {
            this.playerID = playerID;
            this.seenTiles = seenTiles;
            this.focusTile = focusTile;
        }

        public override void Start()
        {
            ClientController.SetActivePlayer(playerID, focusTile);
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

    public class SelectUnitSequence : Sequence
    {
        Coords unit;

        public SelectUnitSequence(Coords unit)
        {
            this.unit = unit;
        }

        public override void Start()
        {
            ClientController.SelectUnit(unit);
            ClientController.Camera.MoveTowards(World.GetTilePosition(unit));
        }

        public override bool Update()
        {
            return !ClientController.Camera.isMovingToTarget;
        }
    }

    public class DeselectUnitSequence : Sequence
    {
        public override void Start()
        {
            ClientController.DeselectUnit();
        }
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
        Coords unit;

        public UnitDieSequence(Coords unit)
        {
            this.unit = unit;
        }

        public override void Start()
        {
            GameState.RemoveUnit(unit);
        }
    }

    public class ExplosionSequence : Sequence
    {
        Coords tile;

        float progress = 0f;

        public ExplosionSequence(Coords tile)
        {
            this.tile = tile;
        }

        public override void Start()
        {
            World.SpawnExplosion(tile);
        }

        public override bool Update()
        {
            progress += Time.deltaTime;
            return progress >= 0.8f;
        }
    }
}
