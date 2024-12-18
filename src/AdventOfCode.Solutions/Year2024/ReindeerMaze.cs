using AdventOfCode.Core.Common;
using AdventOfCode.Core.Utils;

namespace AdventOfCode.Solutions.Year2024;

[AdventOfCode(2024, 16)]
public partial class ReindeerMaze : AdventOfCodeSolution<int>
{
    protected override string Test =>
        """
            ###############
            #.......#....E#
            #.#.###.#.###.#
            #.....#.#...#.#
            #.###.#####.#.#
            #.#.#.......#.#
            #.#.#####.###.#
            #...........#.#
            ###.#.#####.#.#
            #...#.....#.#.#
            #.#.#.###.#.#.#
            #.....#...#.#.#
            #.###.#.#.#.#.#
            #S..#.....#...#
            ###############
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
        var (_, cost) = FindShortestPath(lines);
        return cost;
    }

    protected override int PartTwo(string[] lines)
    {
        var (nodesInAnyShortestPath, _) = FindShortestPath(lines);
        return nodesInAnyShortestPath.Count;
    }

    private static (HashSet<Vector2Int> nodesInAnyShortestPath, int cost) FindShortestPath(
        string[] lines
    )
    {
        var maze = lines.Select(l => l.ToCharArray()).ToArray();
        Vector2Int start = Vector2Int.Zero;
        Vector2Int end = Vector2Int.Zero;

        for (int y = 0; y < maze.Length; y++)
        {
            for (int x = 0; x < maze[y].Length; x++)
            {
                if (maze[y][x] == 'S')
                {
                    start = new Vector2Int(x, y);
                }
                else if (maze[y][x] == 'E')
                {
                    end = new Vector2Int(x, y);
                }
            }
        }

        int n = maze.Length,
            m = maze[0].Length;

        var pq = new PriorityQueue<(Vector2Int pos, Vector2Int dir), int>();
        var distances = new Dictionary<(Vector2Int pos, Vector2Int dir), int>();
        var prev =
            new Dictionary<
                (Vector2Int pos, Vector2Int dir),
                HashSet<(Vector2Int pos, Vector2Int dir)>
            >();

        var starting = Vector2Int.Right;
        distances[(start, starting)] = 0;
        pq.Enqueue((start, starting), 0);

        while (pq.TryDequeue(out var current, out var distance))
        {
            var (pos, dir) = current;

            foreach (var newDir in Directions)
            {
                var newPos = pos + newDir;
                if (!newPos.InBounds(n, m) || maze[newPos.Y][newPos.X] == '#')
                {
                    continue;
                }

                var effort = dir == newDir ? 1 : 1001;
                var newDist = distance + effort;

                if (!distances.TryGetValue((newPos, newDir), out var oldDist) || newDist <= oldDist)
                {
                    distances[(newPos, newDir)] = newDist;
                    pq.Enqueue((newPos, newDir), newDist);

                    if (!prev.TryGetValue((newPos, newDir), out var from))
                    {
                        from = [];
                        prev[(newPos, newDir)] = from;
                    }

                    if (newDist < oldDist)
                    {
                        from.Clear();
                    }

                    from.Add(current);
                }
            }
        }

        // doing this mess because i implemented djikstra for part one
        // and didn't want to rewrite it for part two
        var toCheck = new List<(Vector2Int pos, Vector2Int dir)>();
        var dist = int.MaxValue;

        foreach (var dir in Directions)
        {
            if (distances.TryGetValue((end, dir), out var d) && d < dist)
            {
                dist = d;
                toCheck.Clear();
                toCheck.Add((end, dir));
            }
            else if (d == dist)
            {
                toCheck.Add((end, dir));
            }
        }

        var nodesInAnyShortestPath = new HashSet<Vector2Int>();
        var visited = new HashSet<(Vector2Int pos, Vector2Int dir)>();
        var stack = new Stack<(Vector2Int pos, Vector2Int dir)>();

        foreach (var check in toCheck)
        {
            stack.Push(check);
        }

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (!visited.Add(current))
            {
                continue;
            }

            var (pos, dir) = current;
            nodesInAnyShortestPath.Add(pos);

            if (pos == start)
            {
                continue;
            }

            foreach (var from in prev[(pos, dir)])
            {
                stack.Push(from);
            }
        }

        return (nodesInAnyShortestPath, dist);
    }
}
