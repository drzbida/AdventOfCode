using AdventOfCode.Core.Common;
using AdventOfCode.Core.Utils;

namespace AdventOfCode.Solutions.Year2024;

[AdventOfCode(2024, 12)]
public partial class GardenGroups : AdventOfCodeSolution<int>
{
    protected override string Test =>
        """
            RRRRIICCFF
            RRRRIICCCF
            VVRRRCCFFF
            VVRCCCJFFF
            VVVVCJJCFE
            VVIVCCJJEE
            VVIIICJJEE
            MIIIIIJJEE
            MIIISIJEEE
            MMMISSJEEE
            """;

    private static readonly Vector2Int[] Directions =
    [
        Vector2Int.Up,
        Vector2Int.Down,
        Vector2Int.Left,
        Vector2Int.Right,
    ];

    protected override int PartOne(string[] lines)
    {
        return Solve(lines, false);
    }

    protected override int PartTwo(string[] lines)
    {
        return Solve(lines, true);
    }

    private static int Solve(string[] lines, bool continousSides)
    {
        char[][] garden = lines.Select(l => l.ToCharArray()).ToArray();
        int n = garden.Length,
            m = garden[0].Length;
        var visited = new HashSet<Vector2Int>();
        var result = 0;

        for (var y = 0; y < n; y++)
        {
            for (var x = 0; x < m; x++)
            {
                var pos = new Vector2Int(x, y);
                if (visited.Contains(pos))
                {
                    continue;
                }
                var extremities = new List<Vector2Int>();
                var searchResult = Search(garden, pos, visited, extremities, garden[y][x]);

                var usedResult = extremities.Count;
                if (continousSides)
                {
                    usedResult = searchResult.Y;
                }
                result += searchResult.X * usedResult;
            }
        }

        return result;
    }

    private static Vector2Int Search( // (number of tiles, number of corners)
        char[][] garden,
        Vector2Int pos,
        HashSet<Vector2Int> visited,
        List<Vector2Int> extremities,
        char target
    )
    {
        visited.Add(pos);
        var result = new Vector2Int(1, 0);

        int n = garden.Length,
            m = garden[0].Length;

        foreach (var offset in Directions)
        {
            var newPos = pos + offset;
            if (!newPos.InBounds(n, m) || garden[newPos.Y][newPos.X] != target)
            {
                extremities.Add(pos);
            }
            else if (!visited.Contains(newPos))
            {
                result += Search(garden, newPos, visited, extremities, target);
            }
        }

        var c = 0;
        // OO   OO   -O   O-
        // O-   -O   OO   OO
        var convexDirs = new Vector2Int[] { Vector2Int.Right, Vector2Int.Down, new(1, 1) };

        for (var i = 0; i <= 3; i++)
        {
            var p1 = pos + convexDirs[0];
            var p2 = pos + convexDirs[1];
            var p3 = pos + convexDirs[2];

            var c1 = p1.InBounds(n, m) && garden[p1.Y][p1.X] == target;
            var c2 = p2.InBounds(n, m) && garden[p2.Y][p2.X] == target;
            var c3 = !p3.InBounds(n, m) || garden[p3.Y][p3.X] != target;

            if (c1 && c2 && c3)
            {
                c++;
            }

            convexDirs = convexDirs.Select(c => c.Rotate(90)).ToArray();
        }
        // O-   -O    -   -
        // -     O   -O   O-
        var concaveDirs = new Vector2Int[] { Vector2Int.Right, Vector2Int.Down };

        for (var i = 0; i <= 3; i++)
        {
            var p1 = pos + concaveDirs[0];
            var p2 = pos + concaveDirs[1];

            var c1 = !p1.InBounds(n, m) || garden[p1.Y][p1.X] != target;
            var c2 = !p2.InBounds(n, m) || garden[p2.Y][p2.X] != target;

            if (c1 && c2)
            {
                c++;
            }

            concaveDirs = concaveDirs.Select(c => c.Rotate(90)).ToArray();
        }

        return result + new Vector2Int(0, c);
    }
}
