using AdventOfCode.Core.Common;
using AdventOfCode.Core.Utils;

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
        return lines
            .PartitionBy(string.IsNullOrWhiteSpace)
            .Select(chunk => chunk.Select(int.Parse).ToList())
            .Select(elf => elf.Sum())
            .Max();
    }

    protected override int PartTwo(string[] lines)
    {
        return lines
            .PartitionBy(string.IsNullOrWhiteSpace)
            .Select(chunk => chunk.Select(int.Parse).ToList())
            .Select(elf => elf.Sum())
            .OrderByDescending(x => x)
            .Take(3)
            .Sum();
    }
}
