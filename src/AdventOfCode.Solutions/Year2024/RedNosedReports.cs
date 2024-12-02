using AdventOfCode.Core.Common;

namespace AdventOfCode.Solutions.Year2024;

[AdventOfCode(2024, 2)]
public class RedNosedReports : AdventOfCodeSolution<int>
{
    protected override string Test =>
        """
            7 6 4 2 1
            1 2 7 8 9
            9 7 6 2 1
            1 3 2 4 5
            8 6 4 4 1
            1 3 6 7 9
            """;

    protected override int PartOne(string[] lines)
    {
        var reports = ParseInput(lines);
        return reports.Count(report => IsValidSequence(report));
    }

    protected override int PartTwo(string[] lines)
    {
        var reports = ParseInput(lines);
        // input size is very small so brute force is the shortest
        return reports.Count(report =>
            IsValidSequence(report)
            || Enumerable.Range(0, report.Count).Any(i => IsValidSequence(report, i))
        );
    }

    private static List<List<int>> ParseInput(string[] lines)
    {
        return lines.Select(line => line.Split(' ').Select(int.Parse).ToList()).ToList();
    }

    private static bool IsValidSequence(List<int> report, int? skipIndex = null)
    {
        if (skipIndex.HasValue)
        {
            report = report.Where((_, i) => i != skipIndex).ToList();
        }
        var order = report[0] < report[1] ? 1 : -1;
        return report
            .Zip(report.Skip(1), (a, b) => (b - a) * order)
            .All(diff => diff >= 1 && diff <= 3);
    }
}
