using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// A point in 2D space represented by integer coordinates.
/// </summary>
struct Point2
{
    public int X, Y;

    public static readonly Point2 Zero = new Point2(0, 0);

    /// <summary>
    /// Creates a new 2D point.
    /// </summary>
    /// <param name="x">The X component.</param>
    /// <param name="y">The Y component.</param>
    public Point2(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }

    public static Point2 operator +(Point2 a, Point2 b)
    {
        return new Point2(a.X + b.X, a.Y + b.Y);
    }

    public static Point2 operator -(Point2 a, Point2 b)
    {
        return new Point2(a.X - b.X, a.Y - b.Y);
    }

    public static Point2 operator *(Point2 a, int b)
    {
        return new Point2(a.X * b, a.Y * b);
    }

    public static Point2 operator *(Point2 a, float b)
    {
        return new Point2((int)(a.X * b), (int)(a.Y * b));
    }

    public static Point2 operator /(Point2 a, int b)
    {
        return new Point2(a.X / b, a.Y / b);
    }

    public static Point2 operator /(Point2 a, float b)
    {
        return new Point2((int)(a.X / b), (int)(a.Y / b));
    }

    // implicit conversion from Vector2 to Point2
    public static implicit operator Point2(Vector2 v)
    {
        return new Point2((int)v.X, (int)v.Y);
    }
}
