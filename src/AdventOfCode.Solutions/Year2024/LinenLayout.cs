using AdventOfCode.Core.Common;

namespace AdventOfCode.Solutions.Year2024;

[AdventOfCode(2024, 19)]
public partial class LinenLayout : AdventOfCodeSolution<long>
{
    protected override string Test =>
        """
            r, wr, b, g, bwu, rb, gb, br

            brwrr
            bggr
            gbbr
            rrbgbr
            ubwu
            bwurrg
            brgr
            bbrgwb
            """;

    protected override long PartOne(string[] lines)
    {
        var (patterns, requirements) = ParseInput(lines);
        var memo = new Dictionary<string, long>();
        return requirements.Count(requirement => CountMakeTowel(requirement, patterns, memo) > 0);
    }

    protected override long PartTwo(string[] lines)
    {
        var (patterns, requirements) = ParseInput(lines);
        var memo = new Dictionary<string, long>();
        return requirements.Sum(requirement => CountMakeTowel(requirement, patterns, memo));
    }

    private static long CountMakeTowel(
        string towel,
        List<string> towelPatterns,
        Dictionary<string, long> memo
    )
    {
        if (towel.Length == 0)
            return 1;

        if (memo.TryGetValue(towel, out var value))
            return value;

        long result = 0;

        foreach (var pattern in towelPatterns)
        {
            if (!towel.StartsWith(pattern))
            {
                continue;
            }

            result += CountMakeTowel(towel[pattern.Length..], towelPatterns, memo);
        }

        memo[towel] = result;

        return result;
    }

    private static (List<string> patterns, List<string> requirements) ParseInput(string[] input)
    {
        var patterns = input[0].Split(", ").ToList();
        patterns.Sort((a, b) => b.Length.CompareTo(a.Length));
        var requirements = input[2..].ToList();
        return (patterns, requirements);
    }
}
