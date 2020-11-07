using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;
using System;

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
    public class DelaySequence : Sequence
    {
        float delay;

        public DelaySequence(float delay)
        {
            this.delay = delay;
        }

        public override void Start()
        {
            ClientController.ChangeState(new CameraMoveState());
        }

        public override bool Update()
        {
            delay -= Time.deltaTime;
            return delay <= 0f;
        }

        public override void End()
        {
            ClientController.ChangeState(new DefaultState());
        }
    }
    public class ControlSequence : DelaySequence
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
            base.Start();
            callback();
        }

        public override string ToString()
        {
            return name;
        }
    }
    public class StateUpdateSequence : DelaySequence
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
            base.Start();
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

        bool promptClosed = true;

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
                promptClosed = false;
                PromptUI.Show(state.GetPlayer(player).name + "\nTurn " + turn, state.GetPlayer(player).color, () => promptClosed = true);
                ClientController.CameraController.Translate(World.GetTilePosition(focusTile) - ClientController.CameraController.transform.position);
            }

            ClientController.ChangeActivePlayer(player);
            ClientController.SetState(state);
        }

        public override bool Update()
        {
            return promptClosed;
        }
    }
    public class MoveCameraToUnitSequence : DelaySequence
    {
        int unit;

        public MoveCameraToUnitSequence(int unit) : base(0.3f)
        {
            this.unit = unit;
        }

        public override void Start()
        {
            base.Start();
            ClientController.CameraController.MoveTowards(World.GetTilePosition(ClientController.gameState.GetUnit(unit).tile));
        }

        public override bool Update()
        {
            if(!ClientController.CameraController.isMovingToTarget)
            {
                return base.Update();
            }
            return false;
        }
    }
    public class ExplosionSequence : DelaySequence
    {
        Coords tile;

        public ExplosionSequence(Coords tile) : base(0.8f)
        {
            this.tile = tile;
        }

        public override void Start()
        {
            base.Start();
            World.SpawnExplosion(tile);
        }
    }
}
