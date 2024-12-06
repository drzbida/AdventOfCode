namespace AdventOfCode.Core.Utils;

public readonly struct Vector2Int(int x, int y)
{
    public int X { get; } = x;
    public int Y { get; } = y;

    public static Vector2Int operator +(Vector2Int a, Vector2Int b) => new(a.X + b.X, a.Y + b.Y);

    public static Vector2Int operator -(Vector2Int a, Vector2Int b) => new(a.X - b.X, a.Y - b.Y);

    public static Vector2Int operator *(Vector2Int a, int scalar) =>
        new(a.X * scalar, a.Y * scalar);

    public static Vector2Int operator /(Vector2Int a, int scalar) =>
        new(a.X / scalar, a.Y / scalar);

    public static bool operator ==(Vector2Int a, Vector2Int b) => a.X == b.X && a.Y == b.Y;

    public static bool operator !=(Vector2Int a, Vector2Int b) => !(a == b);

    public static Vector2Int Up => new(0, -1);

    public static Vector2Int Down => new(0, 1);

    public static Vector2Int Left => new(-1, 0);

    public static Vector2Int Right => new(1, 0);

    public static Vector2Int Zero => new(0, 0);

    public override string ToString() => $"({X}, {Y})";

    public override bool Equals(object? obj)
    {
        return obj is Vector2Int other && X == other.X && Y == other.Y;
    }

    public override int GetHashCode() => HashCode.Combine(X, Y);
}

public static class Vector2IntExtensions
{
    public static Vector2Int Rotate(this Vector2Int vector, int degrees)
    {
        var radians = degrees * Math.PI / 180;
        var x = (int)Math.Round(vector.X * Math.Cos(radians) - vector.Y * Math.Sin(radians));
        var y = (int)Math.Round(vector.X * Math.Sin(radians) + vector.Y * Math.Cos(radians));
        return new Vector2Int(x, y);
    }

    public static bool InBounds(this Vector2Int pos, int n, int m) =>
        pos.X >= 0 && pos.X < n && pos.Y >= 0 && pos.Y < m;
}
