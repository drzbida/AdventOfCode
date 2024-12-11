using AdventOfCode.Core.Common;

namespace AdventOfCode.Solutions.Year2024;

[AdventOfCode(2024, 11)]
public partial class PlutonianPebbles : AdventOfCodeSolution<long>
{
    protected override string Test =>
        """
            0 1 10 99 999
            """;

    protected override long PartOne(string[] lines)
    {
        return Solve(lines[0], 25);
    }

    protected override long PartTwo(string[] lines)
    {
        return Solve(lines[0], 75);
    }

    private static long Solve(string input, int blinks)
    {
        var currentDict = new Dictionary<long, long>();
        foreach (var part in input.Split(' '))
        {
            var number = long.Parse(part);
            currentDict[number] = currentDict.GetValueOrDefault(number) + 1;
        }

        for (var i = 0; i < blinks; i++)
        {
            var nextDict = new Dictionary<long, long>();

            foreach (var (key, value) in currentDict)
            {
                if (key == 0)
                {
                    nextDict[1] = nextDict.GetValueOrDefault(1) + value;
                    continue;
                }

                if (Math.Floor(Math.Log10(key) + 1) % 2 == 0)
                {
                    var keyS = key.ToString().AsSpan();
                    var mid = keyS.Length / 2;
                    var firstHalf = long.Parse(keyS[..mid]);
                    var secondHalf = long.Parse(keyS[mid..]);

                    nextDict[firstHalf] = nextDict.GetValueOrDefault(firstHalf) + value;
                    nextDict[secondHalf] = nextDict.GetValueOrDefault(secondHalf) + value;
                    continue;
                }

                var newValue = key * 2024;

                nextDict[newValue] = nextDict.GetValueOrDefault(newValue) + value;
            }

            currentDict = nextDict;
        }

        return currentDict.Sum(x => x.Value);
    }
}
