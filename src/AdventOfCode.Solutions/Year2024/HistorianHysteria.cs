using AdventOfCode.Core.Common;

namespace AdventOfCode.Solutions.Year2024;

[AdventOfCode(2024, 1)]
public class HistorianHysteria : AdventOfCodeSolution<int>
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

    protected override int PartOne(string[] lines)
    {
        var (left, right) = ParseInput(lines);
        left.Sort();
        right.Sort();

        return Enumerable.Zip(left, right).Select(x => Math.Abs(x.First - x.Second)).Sum();
    }

    protected override int PartTwo(string[] lines)
    {
        var (left, right) = ParseInput(lines);
        var rightMap = right.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());

        return left.Select(x => x * rightMap.GetValueOrDefault(x, 0)).Sum();
    }

    private static (List<int> left, List<int> right) ParseInput(string[] lines)
    {
        var left = new List<int>();
        var right = new List<int>();

        foreach (var line in lines)
        {
            var parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            left.Add(int.Parse(parts[0]));
            right.Add(int.Parse(parts[1]));
        }

        return (left, right);
    }
}
