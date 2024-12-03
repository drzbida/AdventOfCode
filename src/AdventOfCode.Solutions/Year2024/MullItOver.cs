using System.Text.RegularExpressions;
using AdventOfCode.Core.Common;

namespace AdventOfCode.Solutions.Year2024;

[AdventOfCode(2024, 3)]
public partial class MullItOver : AdventOfCodeSolution<int>
{
    protected override string Test =>
        """
            xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))
            """;

    protected override int PartOne(string[] lines)
    {
        return MultiplyRegex()
            .Matches(string.Concat(lines))
            .Sum(m => int.Parse(m.Groups[1].Value) * int.Parse(m.Groups[2].Value));
    }

    protected override int PartTwo(string[] lines)
    {
        return MultiplyRegex()
            .Matches(DisableRegex().Replace(string.Concat(lines), "XX"))
            .Sum(m => int.Parse(m.Groups[1].Value) * int.Parse(m.Groups[2].Value));
    }

    [GeneratedRegex(@"mul\((\d+),(\d+)\)")]
    private static partial Regex MultiplyRegex();

    [GeneratedRegex(@"don't\(\).*?(do\(\)|$)")]
    private static partial Regex DisableRegex();
}
