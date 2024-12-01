using AdventOfCode.Core.Common;

namespace AdventOfCode.Solutions.Year2024;

[AdventOfCode(2024, 1)]
public class Day1 : AdventOfCodeSolution
{
    protected override string Test =>
        """
            3   4
            4   3
            2   5
            1   3
            3   9
            3   3
            """;

    protected override string PartOne(string[] lines)
    {
        return "1";
    }

    protected override string PartTwo(string[] lines)
    {
        throw new NotImplementedException();
    }
}
