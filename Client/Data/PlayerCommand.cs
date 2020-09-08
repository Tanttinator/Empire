using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Server;

public abstract class PlayerCommand
{
    public abstract void Execute(Human player, Unit unit);
}

public class CommandMove : PlayerCommand
{
    public Coords coords;

    public CommandMove(Coords coords)
    {
        this.coords = coords;
    }

    public override void Execute(Human player, Unit unit)
    {
        if (unit == null) return;
        unit.SetTarget(World.GetTile(coords));
    }
}

public class CommandMoveDir : PlayerCommand
{
    public Direction dir;

    public CommandMoveDir(Direction dir)
    {
        this.dir = dir;
    }

    public override void Execute(Human player, Unit unit)
    {
        World.GetTile(unit.tile.coords.Neighbor(dir))?.Interact(unit);
    }
}

public class CommandWait : PlayerCommand
{
    public override void Execute(Human player, Unit unit)
    {
        player.NextUnit();
    }
}

public class CommandSleep : PlayerCommand
{
    public override void Execute(Human player, Unit unit)
    {
        unit.SetSleeping(true);
    }
}
