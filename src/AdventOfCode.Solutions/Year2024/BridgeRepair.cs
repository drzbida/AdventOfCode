using System.Text.RegularExpressions;
using AdventOfCode.Core.Common;

namespace AdventOfCode.Solutions.Year2024;

[AdventOfCode(2024, 7)]
public partial class BridgeRepair : AdventOfCodeSolution<long>
{
    protected override string Test =>
        """
            190: 10 19
            3267: 81 40 27
            83: 17 5
            156: 15 6
            7290: 6 8 6 15
            161011: 16 10 13
            192: 17 8 14
            21037: 9 7 18 13
            292: 11 6 16 20
            """;

    protected override long PartOne(string[] lines)
    {
        return lines
            .AsParallel()
            .Sum(line =>
            {
                var match = InputRegex().Match(line);
                var target = long.Parse(match.Groups[1].Value);
                var elements = match.Groups[2].Value.Split(' ').Select(long.Parse).ToArray();

                return CanReachTargetBackward(target, elements, elements.Length - 1) ? target : 0;
            });
    }

    protected override long PartTwo(string[] lines)
    {
        // ~900 microseconds
        return lines
            .AsParallel()
            .Sum(line =>
            {
                var match = InputRegex().Match(line);
                var target = long.Parse(match.Groups[1].Value);
                var elements = match.Groups[2].Value.Split(' ').Select(long.Parse).ToArray();

                return CanReachTargetBackward(target, elements, elements.Length - 1, true)
                    ? target
                    : 0;
            });
    }

    private static bool CanReachTargetBackward(
        long target,
        long[] elements,
        int index,
        bool hasConcatenate = false
    )
    {
        if (index < 0)
        {
            return target == 0;
        }
        var current = elements[index];

        // multiply valid
        if (target % current == 0)
        {
            if (CanReachTargetBackward(target / current, elements, index - 1, hasConcatenate))
            {
                return true;
            }
        }

        // concatenate valid
        if (hasConcatenate)
        {
            var targetStr = target.ToString();
            var currentStr = current.ToString();

            if (targetStr.EndsWith(currentStr))
            {
                var nextTarget = targetStr[..^currentStr.Length];
                if (!long.TryParse(nextTarget, out var parsedNextTarget))
                {
                    parsedNextTarget = 0;
                }

                if (CanReachTargetBackward(parsedNextTarget, elements, index - 1, hasConcatenate))
                {
                    return true;
                }
            }
        }

        // add valid
        return CanReachTargetBackward(target - current, elements, index - 1, hasConcatenate);
    }

    [GeneratedRegex(@"(\d+): (.*)")]
    private static partial Regex InputRegex();
}
