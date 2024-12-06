using AdventOfCode.Core.Common;
using AdventOfCode.Core.Utils;

namespace AdventOfCode.Solutions.Year2024;

[AdventOfCode(2024, 4)]
public class CeresSearch : AdventOfCodeSolution<int>
{
    protected override string Test =>
        """
            MMMSXXMASM
            MSAMXMSMSA
            AMXSXMAAMM
            MSAMASMSMX
            XMASAMXAMM
            XXAMMXXAMA
            SMSMSASXSS
            SAXAMASAAA
            MAMMMXMMMM
            MXMXAXMASX
            """;

    private static readonly Vector2Int[] Directions =
    [
        new(-1, 0),
        new(1, 0),
        new(0, -1),
        new(0, 1),
        new(-1, -1),
        new(-1, 1),
        new(1, -1),
        new(1, 1),
    ];

    private static readonly (Vector2Int, Vector2Int)[] CrossDirections =
    [
        (new Vector2Int(-1, -1), new Vector2Int(1, 1)),
        (new Vector2Int(1, -1), new Vector2Int(-1, 1)),
    ];

    private static readonly string XMAS = "XMAS";

    protected override int PartOne(string[] lines)
    {
        var mat = lines.Select(line => line.ToCharArray()).ToArray();

        return GetCharIndexes(mat, 'X')
            .Sum(pos => Directions.Sum(dir => CountXMAS(mat, pos, string.Empty, dir)));
    }

    protected override int PartTwo(string[] lines)
    {
        var mat = lines.Select(line => line.ToCharArray()).ToArray();

        return GetCharIndexes(mat, 'A').Sum(pos => IsCrossMas(mat, pos));
    }

    private static IEnumerable<Vector2Int> GetCharIndexes(char[][] mat, char c)
    {
        for (int i = 0; i < mat.Length; i++)
        {
            for (int j = 0; j < mat[0].Length; j++)
            {
                if (mat[i][j] == c)
                {
                    yield return new Vector2Int(i, j);
                }
            }
        }
    }

    private static int CountXMAS(char[][] mat, Vector2Int pos, string current, Vector2Int direction)
    {
        current += mat[pos.X][pos.Y];

        if (current.Length > XMAS.Length || !XMAS.StartsWith(current))
        {
            return 0;
        }

        if (current == XMAS)
        {
            return 1;
        }

        var nextPos = pos + direction;

        if (!nextPos.InBounds(mat.Length, mat[0].Length))
        {
            return 0;
        }

        return CountXMAS(mat, nextPos, current, direction);
    }

    private static int IsCrossMas(char[][] mat, Vector2Int center)
    {
        var n = mat.Length;
        var m = mat[0].Length;

        foreach (var (dir1, dir2) in CrossDirections)
        {
            var pos1 = center + dir1;
            var pos2 = center + dir2;

            if (!pos1.InBounds(n, m) || !pos2.InBounds(n, m))
            {
                return 0;
            }

            var result = $"{mat[pos1.X][pos1.Y]}{mat[center.X][center.Y]}{mat[pos2.X][pos2.Y]}";

            if (result != "MAS" && result != "SAM")
            {
                return 0;
            }
        }

        return 1;
    }
}
