namespace AdventOfCode.Core.Utils;

public readonly struct Vector2Int(int x, int y)
{
    public int X { get; } = x;
    public int Y { get; } = y;

    public static Vector2Int operator +(Vector2Int a, Vector2Int b) => new(a.X + b.X, a.Y + b.Y);
}
