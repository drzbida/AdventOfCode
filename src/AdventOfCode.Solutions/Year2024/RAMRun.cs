using AdventOfCode.Core.Common;
using AdventOfCode.Core.Utils;

namespace AdventOfCode.Solutions.Year2024;

[AdventOfCode(2024, 18)]
public partial class RAMRun : AdventOfCodeSolution<string>
{
    protected override string Test =>
        """
            5,4
            4,2
            4,5
            3,0
            2,1
            6,3
            2,4
            1,5
            0,6
            3,3
            2,6
            5,1
            1,2
            5,5
            2,5
            6,5
            1,4
            0,4
            6,4
            1,1
            6,1
            1,0
            0,5
            1,6
            2,0
            """;

    protected override string PartOne(string[] lines)
    {
        int N = 71,
            elapsed = 1024;

        if (lines[0] == "5,4")
        {
            N = 7;
            elapsed = 12;
        }

        var grid = new bool[N, N];

        for (var i = 0; i < elapsed; i++)
        {
            var inp = lines[i].Split(',').Select(int.Parse).ToArray();
            grid[inp[1], inp[0]] = true;
        }

        return FindShortestPath(grid, new Vector2Int(0, 0), new Vector2Int(N - 1, N - 1), N)
            .ToString();
    }

    protected override string PartTwo(string[] lines)
    {
        int N = 71;

        if (lines[0] == "5,4")
        {
            N = 7;
        }

        var grid = new bool[N, N];

        foreach (var line in lines)
        {
            var inp = line.Split(',').Select(int.Parse).ToArray();
            grid[inp[1], inp[0]] = true;
            var res = FindShortestPath(grid, new Vector2Int(0, 0), new Vector2Int(N - 1, N - 1), N);
            if (res == -1)
            {
                return line;
            }
        }

        return "-1";
    }

    private readonly List<Vector2Int> Directions =
    [
        Vector2Int.Right,
        Vector2Int.Left,
        Vector2Int.Up,
        Vector2Int.Down,
    ];

    private int FindShortestPath(bool[,] grid, Vector2Int start, Vector2Int end, int N)
    {
        var q = new Queue<(Vector2Int, int)>();
        var distances = new Dictionary<Vector2Int, int>();

        q.Enqueue((start, 0));

        while (q.Count > 0)
        {
            var (elem, dist) = q.Dequeue();

            foreach (var d in Directions)
            {
                var next = elem + d;

                if (!next.InBounds(N, N) || grid[next.Y, next.X])
                    continue;

                var newDist = dist + 1;

                if (!distances.TryGetValue(next, out var oldDist) || newDist < oldDist)
                {
                    distances[next] = newDist;
                    q.Enqueue((next, newDist));
                }
            }
        }

        return distances.GetValueOrDefault(end, -1);
    }
}
