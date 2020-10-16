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
            this.delay = 0f;
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

        public ControlSequence(Action callback, float delay = 0f) : base(delay)
        {
            this.callback = callback;
        }

        public override void Start()
        {
            callback();
        }
    }
    public class MoveCameraToUnitSequence : Sequence
    {
        public MoveCameraToUnitSequence() : base(0.3f)
        {

        }

        public override void Start()
        {
            ClientController.Camera.MoveTowards(World.GetTilePosition(ClientController.ActiveUnit.tile));
        }

        public override bool Update()
        {
            return !ClientController.Camera.isMovingToTarget;
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
