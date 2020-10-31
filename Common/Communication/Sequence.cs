using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;
using System;

namespace Common
{
    public class Sequence
    {
        float delay;

        public Sequence(float delay)
        {
            this.delay = delay;
        }

        public Sequence()
        {
            delay = 0f;
        }

        public virtual void Start()
        {

        }

        public virtual bool Update()
        {
            delay -= Time.deltaTime;
            return delay <= 0f;
        }

        public virtual void End()
        {

        }
    }
    public class ControlSequence : Sequence
    {
        Action callback;
        string name;

        public ControlSequence(string name, Action callback, float delay = 0f) : base(delay)
        {
            this.name = name;
            this.callback = callback;
        }

        public override void Start()
        {
            callback();
        }

        public override string ToString()
        {
            return name;
        }
    }
    public class StateUpdateSequence : Sequence
    {
        int player;
        GameState state;

        public StateUpdateSequence(int player, GameState state, float delay = 0f) : base(delay)
        {
            this.player = player;
            this.state = state;
        }

        public override void Start()
        {
            if (ClientController.activePlayer == player) ClientController.SetState(state);
        }

        public override bool Update()
        {
            return player != ClientController.activePlayer || base.Update();
        }
    }
    public class StartTurnSequence : Sequence
    {
        int player;
        int turn;
        GameState state;
        Coords focusTile;

        bool promptClosed = false;

        public StartTurnSequence(int player, int turn, GameState state, Coords focusTile) : base()
        {
            this.player = player;
            this.turn = turn;
            this.state = state;
            this.focusTile = focusTile;
        }

        public override void Start()
        {
            if (ClientController.activePlayer != player)
            {
                PromptUI.Show(state.GetPlayer(player).name + "\nTurn " + turn, state.GetPlayer(player).color, () => promptClosed = true);
                ClientController.Camera.Translate(World.GetTilePosition(focusTile) - ClientController.Camera.transform.position);
            }

            ClientController.ChangeActivePlayer(player);
            ClientController.SetState(state);
        }

        public override bool Update()
        {
            return promptClosed;
        }
    }
    public class MoveCameraToUnitSequence : Sequence
    {
        int unit;

        public MoveCameraToUnitSequence(int unit) : base(0.3f)
        {
            this.unit = unit;
        }

        public override void Start()
        {
            ClientController.Camera.MoveTowards(World.GetTilePosition(ClientController.currentState.GetUnit(unit).tile));
        }

        public override bool Update()
        {
            if(!ClientController.Camera.isMovingToTarget)
            {
                return base.Update();
            }
            return false;
        }
    }
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
