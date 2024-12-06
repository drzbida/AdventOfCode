using AdventOfCode.Core.Common;
using AdventOfCode.Core.Utils;

namespace AdventOfCode.Solutions.Year2024;

[AdventOfCode(2024, 6)]
public class GuardGallivant : AdventOfCodeSolution<int>
{
    private static class MapTile
    {
        public static char Wall = '#';
        public static char Empty = '.';
        public static char Start = '^';
    }

    protected override string Test =>
        """
            ....#.....
            .........#
            ..........
            ..#.......
            .......#..
            ..........
            .#..^.....
            ........#.
            #.........
            ......#...
            """;

    protected override int PartOne(string[] lines)
    {
        var map = lines.Select(line => line.ToCharArray()).ToArray();

        var currentPosition = GetStartingPosition(map);
        var currentDirection = Vector2Int.Up;
        var n = map.Length;
        var m = map[0].Length;

        var (combinations, _) = GetPositions(map, currentPosition, currentDirection);

        return combinations.Select(c => c.position).ToHashSet().Count;
    }

    protected override int PartTwo(string[] lines)
    {
        // got to around 300ms with Release; further optimizations would decrease readability too much
        var map = lines.Select(line => line.ToCharArray()).ToArray();

        var currentPosition = GetStartingPosition(map);
        var currentDirection = Vector2Int.Up;
        var n = map.Length;
        var m = map[0].Length;

        var (combinations, _) = GetPositions(map, currentPosition, currentDirection);
        var path = combinations.Select(c => c.position).ToHashSet();

        var ways = 0;

        Parallel.ForEach(
            path,
            position =>
            {
                if (position == currentPosition)
                    return;

                var (newPath, stuck) = GetPositions(
                    map,
                    currentPosition,
                    currentDirection,
                    position
                );

                if (stuck)
                {
                    Interlocked.Increment(ref ways);
                }
            }
        );

        return ways;
    }

    private static Vector2Int GetStartingPosition(char[][] map)
    {
        for (int y = 0; y < map.Length; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                if (map[y][x] == MapTile.Start)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        throw new InvalidOperationException("Start position not found");
    }

    private static (
        HashSet<(Vector2Int position, Vector2Int direction)> path,
        bool stuck
    ) GetPositions(
        char[][] map,
        Vector2Int start,
        Vector2Int originalDirection,
        Vector2Int extraWall = default
    )
    {
        var currentPosition = start;
        var n = map.Length;
        var m = map[0].Length;
        var currentDirection = originalDirection;

        var combinations = new HashSet<(Vector2Int, Vector2Int)>();

        while (true)
        {
            if (combinations.Contains((currentPosition, currentDirection)))
            {
                return (combinations, true);
            }
            combinations.Add((currentPosition, currentDirection));

            var reverseDirection = currentDirection.Rotate(180);

            while (currentDirection != reverseDirection)
            {
                var nextPosition = currentPosition + currentDirection;

                if (!nextPosition.InBounds(n, m))
                {
                    return (combinations, false);
                }

                if (
                    map[nextPosition.Y][nextPosition.X] != MapTile.Wall
                    && nextPosition != extraWall
                )
                {
                    break;
                }

                currentDirection = currentDirection.Rotate(90);
            }

            currentPosition += currentDirection;
        }
    }
}
