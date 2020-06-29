using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Data structure to represent direction in a 2D grid.
/// </summary>
public class Direction
{
    public Coords offset { get; protected set; }

    int id = -1;
    int ID
    {
        get
        {
            if(id == -1)
            {
                id = Array.IndexOf(directions, this);
            }
            return id;
        }
    }

    public Direction Clockwise => directions[(ID + 1) % directions.Length];
    public Direction CounterClockwise => directions[(ID - 1 + directions.Length) % directions.Length];
    public Direction Opposite => directions[(ID + directions.Length / 2) % directions.Length];

    public Direction(Coords offset)
    {
        this.offset = offset;
    }

    public static Direction NORTH = new Direction(new Coords(0, 1));
    public static Direction NORTH_EAST = new Direction(new Coords(1, 1));
    public static Direction EAST = new Direction(new Coords(1, 0));
    public static Direction SOUTH_EAST = new Direction(new Coords(1, -1));
    public static Direction SOUTH = new Direction(new Coords(0, -1));
    public static Direction SOUTH_WEST = new Direction(new Coords(-1, -1));
    public static Direction WEST = new Direction(new Coords(-1, 0));
    public static Direction NORTH_WEST = new Direction(new Coords(-1, 1));

    public static Direction[] directions = new Direction[]
    {
        NORTH, NORTH_EAST, EAST, SOUTH_EAST, SOUTH, SOUTH_WEST, WEST, NORTH_WEST
    };
}
