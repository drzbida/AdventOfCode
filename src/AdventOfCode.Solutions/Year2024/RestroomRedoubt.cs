using System.Text.RegularExpressions;
using AdventOfCode.Core.Common;
using AdventOfCode.Core.Utils;

namespace AdventOfCode.Solutions.Year2024;

[AdventOfCode(2024, 14)]
public partial class RestroomRedoubt : AdventOfCodeSolution<int>
{
    protected override string Test =>
        """
            p=0,4 v=3,-3
            p=6,3 v=-1,-3
            p=10,3 v=-1,2
            p=2,0 v=2,-1
            p=0,0 v=1,3
            p=3,0 v=-2,-2
            p=7,6 v=-1,-3
            p=3,0 v=-1,-2
            p=9,3 v=2,3
            p=7,3 v=-1,2
            p=2,4 v=2,-3
            p=9,5 v=-3,-3
            """;

    protected override int PartOne(string[] lines)
    {
        int width = 101,
            height = 103;

        var seconds = 100;

        if (lines[0].StartsWith("p=0,4"))
        { // Test case
            width = 11;
            height = 7;
        }

        var lineRegex = LineRegex();

        var middleX = width / 2;
        var middleY = height / 2;

        var quadrants = new int[4];
        foreach (var line in lines)
        {
            var match = lineRegex.Match(line);

            var initialX = int.Parse(match.Groups[1].Value);
            var initialY = int.Parse(match.Groups[2].Value);

            var velocityX = int.Parse(match.Groups[3].Value);
            var velocityY = int.Parse(match.Groups[4].Value);

            var x = initialX + velocityX * seconds;
            var y = initialY + velocityY * seconds;

            x = ((x % width) + width) % width;
            y = ((y % height) + height) % height;

            if (x == middleX || y == middleY)
            {
                continue;
            }

            var q = (x < middleX ? 0 : 1) + (y < middleY ? 0 : 2);
            quadrants[q]++;
        }
        return quadrants.Aggregate(1, (product, count) => product * count);
    }

    protected override int PartTwo(string[] lines)
    {
        if (lines[0].StartsWith("p=0,4"))
        { // Test case, don't think it has a solution
            return -1;
        }

        int width = 101,
            height = 103;

        var maxSeconds = 15000;

        var lineRegex = LineRegex();
        var inputs = new List<(Vector2Int position, Vector2Int velocity)>();

        foreach (var line in lines)
        {
            var match = lineRegex.Match(line);

            var initialX = int.Parse(match.Groups[1].Value);
            var initialY = int.Parse(match.Groups[2].Value);

            var velocityX = int.Parse(match.Groups[3].Value);
            var velocityY = int.Parse(match.Groups[4].Value);

            inputs.Add((new Vector2Int(initialX, initialY), new Vector2Int(velocityX, velocityY)));
        }

        var result = -1;

        Parallel.For(
            1,
            maxSeconds + 1,
            (second, state) =>
            {
                var map = new int[height, width];

                foreach (var (position, velocity) in inputs)
                {
                    var final = position + velocity * second;

                    var x = ((final.X % width) + width) % width;
                    var y = ((final.Y % height) + height) % height;

                    map[y, x] = 1;
                }

                if (HasBigObject(map, height, width, 25))
                {
                    /*Console.WriteLine();*/
                    /*for (var y = 0; y < height; y++)*/
                    /*{*/
                    /*    for (var x = 0; x < width; x++)*/
                    /*    {*/
                    /*        if (map[y, x] == 1)*/
                    /*        {*/
                    /*            Console.Write("#");*/
                    /*        }*/
                    /*        else*/
                    /*        {*/
                    /*            Console.Write(".");*/
                    /*        }*/
                    /*    }*/
                    /**/
                    /*    Console.WriteLine();*/
                    /*}*/
                    /**/
                    /*Console.WriteLine();*/
                    result = second;
                    state.Stop();
                }
            }
        );
        return result;
    }

    private static readonly Vector2Int[] Directions =
    [
        Vector2Int.Up,
        Vector2Int.Right,
        Vector2Int.Down,
        Vector2Int.Left,
    ];

    private static bool HasBigObject(int[,] map, int height, int width, int size)
    {
        var visited = new HashSet<Vector2Int>();
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var pos = new Vector2Int(x, y);
                if (map[y, x] == 0 || visited.Contains(pos))
                {
                    continue;
                }

                var count = FindObjectSize(map, height, width, pos, visited);

                if (count >= size)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static int FindObjectSize(
        int[,] map,
        int height,
        int width,
        Vector2Int pos,
        HashSet<Vector2Int> visited
    )
    {
        var stack = new Stack<Vector2Int>();
        stack.Push(pos);
        var count = 0;

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            visited.Add(current);
            count++;

            foreach (var dxy in Directions)
            {
                var next = current + dxy;

                if (
                    !next.InBounds(width, height)
                    || visited.Contains(next)
                    || map[next.Y, next.X] == 0
                )
                {
                    continue;
                }

                stack.Push(next);
            }
        }

        return count;
    }

    [GeneratedRegex(@"p=(\d+),(\d+) v=(\d+|-\d+),(\d+|-\d+)")]
    private static partial Regex LineRegex();
}
