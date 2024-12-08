using AdventOfCode.Core.Common;
using AdventOfCode.Core.Utils;

namespace AdventOfCode.Solutions.Year2024;

[AdventOfCode(2024, 8)]
public class ResonantCollinearity : AdventOfCodeSolution<int>
{
    protected override string Test =>
        """
            ............
            ........0...
            .....0......
            .......0....
            ....0.......
            ......A.....
            ............
            ............
            ........A...
            .........A..
            ............
            ............
            """;

    protected override int PartOne(string[] lines)
    {
        var allAntennas = GetPositions(lines);
        var n = lines.Length;
        var m = lines[0].Length;

        var antinodes = new HashSet<Vector2Int>();

        foreach (var antennas in allAntennas.Values)
        {
            var pairs = antennas.SelectMany((a1, i) => antennas.Skip(i + 1), (a1, a2) => (a1, a2));
            foreach (var (a1, a2) in pairs)
            {
                var line = GetLine(a1, a1 - a2, n, m)
                    .Where(v =>
                    {
                        var d1 = new Vector2Int(Math.Abs(a1.X - v.X), Math.Abs(a1.Y - v.Y));
                        var d2 = new Vector2Int(Math.Abs(a2.X - v.X), Math.Abs(a2.Y - v.Y));

                        return d1 == d2 * 2 || d2 == d1 * 2;
                    });

                antinodes.UnionWith(line);
            }
        }

        return antinodes.Count;
    }

    protected override int PartTwo(string[] lines)
    {
        var allAntennas = GetPositions(lines);
        var n = lines.Length;
        var m = lines[0].Length;

        var antinodes = new HashSet<Vector2Int>();

        foreach (var antennas in allAntennas.Values)
        {
            var pairs = antennas.SelectMany((a1, i) => antennas.Skip(i + 1), (a1, a2) => (a1, a2));

            foreach (var (a1, a2) in pairs)
            {
                antinodes.UnionWith(GetLine(a1, a1 - a2, n, m));
            }
        }

        return antinodes.Count;
    }

    private static List<Vector2Int> GetLine(Vector2Int start, Vector2Int diff, int n, int m)
    {
        var line = new List<Vector2Int>();
        for (var pos = start; pos.InBounds(n, m); pos += diff)
        {
            line.Add(pos);
        }

        for (var pos = start - diff; pos.InBounds(n, m); pos -= diff)
        {
            line.Add(pos);
        }

        return line;
    }

    private static Dictionary<char, List<Vector2Int>> GetPositions(string[] lines) =>
        lines
            .SelectMany((line, y) => line.Select((c, x) => (c, pos: new Vector2Int(x, y))))
            .Where(item => item.c != '.')
            .GroupBy(item => item.c)
            .ToDictionary(group => group.Key, group => group.Select(item => item.pos).ToList());
}
