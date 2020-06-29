using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data structure to represent points in a 2D grid.
/// </summary>
public struct Coords
{
    public int x { get; private set; }
    public int y { get; private set; }

    public Coords(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    /// <summary>
    /// Returns the coordinates one step along the given direction.
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public Coords Neighbor(Direction dir) => this + dir.offset;

    public static implicit operator Coords(Vector2 v2)
    {
        return new Coords((int)v2.x, (int)v2.y);
    }

    public static implicit operator Vector2(Coords c)
    {
        return new Vector2(c.x, c.y);
    }

    public static Coords operator +(Coords a, Coords b)
    {
        return new Coords(a.x + b.x, a.y + b.y);
    }

    public static Coords operator -(Coords a, Coords b)
    {
        return new Coords(a.x - b.x, a.y - b.y);
    }

    public static bool operator ==(Coords a, Coords b)
    {
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(Coords a, Coords b)
    {
        return a.x != b.x || a.y != b.y;
    }

    public override bool Equals(object obj)
    {
        if (obj is Coords c) return c == this;
        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }
}
