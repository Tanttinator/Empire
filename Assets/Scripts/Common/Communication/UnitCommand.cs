using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Server;

namespace Common
{
    public abstract class UnitCommand
    {
        public abstract void Execute(Human human, Unit unit);
    }

    public class CommandMove : UnitCommand
    {
        Coords tile;

        public CommandMove(Coords tile)
        {
            this.tile = tile;
        }

        public override void Execute(Human human, Unit unit)
        {
            unit.SetTarget(World.GetTile(tile));
        }
    }

    public class CommandWait : UnitCommand
    {
        public override void Execute(Human human, Unit unit)
        {
            human.SetUnitInactive(unit);
        }
    }

    public class CommandSleep : UnitCommand
    {
        public override void Execute(Human human, Unit unit)
        {
            unit.SetSleeping(true);
        }
    }

    public class CommandUnload : UnitCommand
    {
        public override void Execute(Human human, Unit unit)
        {
            human.UnloadUnit(unit);
        }
    }
}
