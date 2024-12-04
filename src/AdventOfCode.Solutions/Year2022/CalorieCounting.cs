using AdventOfCode.Core.Common;

namespace AdventOfCode.Solutions.Year2022;

[AdventOfCode(2022, 1)]
public class HistorianHysteria : AdventOfCodeSolution<int>
{
    protected override string Test =>
        """
            1000
            2000
            3000

            4000

            5000
            6000

            7000
            8000
            9000

            10000
            """;

    protected override int PartOne(string[] lines)
    {
        return ChunkByEmptyLine(lines).Select(elf => elf.Sum()).Max();
    }

    protected override int PartTwo(string[] lines)
    {
        return ChunkByEmptyLine(lines)
            .Select(elf => elf.Sum())
            .OrderByDescending(x => x)
            .Take(3)
            .Sum();
    }

    public static IEnumerable<List<int>> ChunkByEmptyLine(string[] lines) =>
        lines.Aggregate(
            new List<List<int>> { new() },
            (acc, line) =>
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    acc.Add([]);
                }
                else
                {
                    acc[^1].Add(int.Parse(line));
                }
                return acc;
            }
        );
}
