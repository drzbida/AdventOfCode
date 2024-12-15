using System.Text.RegularExpressions;
using AdventOfCode.Core.Common;
using AdventOfCode.Core.Utils;

namespace AdventOfCode.Solutions.Year2024;

[AdventOfCode(2024, 13)]
public partial class ClawContraption : AdventOfCodeSolution<long>
{
    protected override string Test =>
        """
            Button A: X+94, Y+34
            Button B: X+22, Y+67
            Prize: X=8400, Y=5400

            Button A: X+26, Y+66
            Button B: X+67, Y+21
            Prize: X=12748, Y=12176

            Button A: X+17, Y+86
            Button B: X+84, Y+37
            Prize: X=7870, Y=6450

            Button A: X+69, Y+23
            Button B: X+27, Y+71
            Prize: X=18641, Y=10279
            """;

    private readonly long Modifier = 10_000_000_000_000;

    protected override long PartOne(string[] lines)
    {
        return Solve(lines, false);
    }

    protected override long PartTwo(string[] lines)
    {
        return Solve(lines, true);
    }

    private long Solve(string[] lines, bool isPartTwo)
    {
        var groups = lines.PartitionBy(string.IsNullOrWhiteSpace);
        var buttonRegex = ButtonRegex();
        var prizeRegex = PrizeRegex();
        var result = 0L;

        foreach (var group in groups)
        {
            var buttonA = buttonRegex.Match(group[0]);
            var buttonB = buttonRegex.Match(group[1]);
            var prize = prizeRegex.Match(group[2]);

            var xA = long.Parse(buttonA.Groups[1].Value);
            var yA = long.Parse(buttonA.Groups[2].Value);

            var xB = long.Parse(buttonB.Groups[1].Value);
            var yB = long.Parse(buttonB.Groups[2].Value);

            var xPrize = long.Parse(prize.Groups[1].Value);
            var yPrize = long.Parse(prize.Groups[2].Value);

            if (isPartTwo)
            {
                xPrize += Modifier;
                yPrize += Modifier;
            }

            var det = xA * yB - xB * yA;

            var detX = xPrize * yB - xB * yPrize;
            var detY = xA * yPrize - xPrize * yA;

            if (detX % det != 0 || detY % det != 0)
            {
                continue;
            }

            var a = detX / det;
            var b = detY / det;

            result += 3 * a + b;
        }

        return result;
    }

    [GeneratedRegex(@"Button .: X\+(\d+), Y\+(\d+)")]
    private static partial Regex ButtonRegex();

    [GeneratedRegex(@"Prize: X=(\d+), Y=(\d+)")]
    private static partial Regex PrizeRegex();
}
