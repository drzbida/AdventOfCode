using AdventOfCode.Core.Common;
using AdventOfCode.Core.Utils;

namespace AdventOfCode.Solutions.Year2024;

[AdventOfCode(2024, 10)]
public partial class HoofIt : AdventOfCodeSolution<int>
{
    protected override string Test =>
        """
            89010123
            78121874
            87430965
            96549874
            45678903
            32019012
            01329801
            10456732
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

    private static int Solve(string[] lines, bool allowDuplicates)
    {
        int n = lines.Length,
            m = lines[0].Length;
        int[][] map = new int[n][];

        var zeros = new List<Vector2Int>();
        for (int i = 0; i < n; i++)
        {
            map[i] = new int[m];
            for (int j = 0; j < m; j++)
            {
                var value = lines[i][j] - '0';
                map[i][j] = value;
                if (value == 0)
                {
                    zeros.Add(new Vector2Int(j, i));
                }
            }
        }

        return zeros.Sum(pos => FindNine(map, pos, allowDuplicates ? null : []));
    }

    private static int FindNine(int[][] map, Vector2Int pos, HashSet<Vector2Int>? nines)
    {
        var current = map[pos.Y][pos.X];

        if (current == 9)
        {
            if (nines != null && nines.Contains(pos))
            {
                return 0;
            }
            nines?.Add(pos);
            return 1;
        }

        int result = 0,
            next = current + 1,
            n = map.Length,
            m = map[0].Length;

        foreach (var offset in Directions)
        {
            var nextPos = pos + offset;
            if (nextPos.InBounds(n, m) && map[nextPos.Y][nextPos.X] == next)
            {
                result += FindNine(map, nextPos, nines);
            }
        }

        return result;
    }
}
