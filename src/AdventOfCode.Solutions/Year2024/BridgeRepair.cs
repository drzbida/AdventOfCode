using System.Numerics;
using System.Text.RegularExpressions;
using AdventOfCode.Core.Common;
using AdventOfCode.Core.Utils;

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
        long total = 0;
        foreach (var line in lines)
        {
            var match = InputRegex().Match(line);
            var value = long.Parse(match.Groups[1].Value);
            var elements = match.Groups[2].Value.Split(' ').Select(long.Parse).ToArray();
            var num_bits = elements.Length - 1;

            var max_number = (2 << num_bits) - 1;

            var found = false;

            for (var current_number = 0; current_number < max_number; current_number++)
            {
                var sum = elements[0];

                for (var i = 1; i < elements.Length; i++)
                {
                    if ((current_number & (1 << i)) != 0)
                    {
                        sum += elements[i];
                    }
                    else
                    {
                        sum *= elements[i];
                    }
                }

                if (sum == value)
                {
                    found = true;
                    break;
                }
            }

            if (found)
            {
                total += value;
            }
        }
        return total;
    }

    public static string LongToString(long value, int toBase)
    {
        string result = string.Empty;
        do
        {
            result = "0123456789ABCDEF"[(int)value % toBase] + result;
            value /= toBase;
        } while (value > 0);

        return result;
    }

    protected override long PartTwo(string[] lines)
    {
        long total = 0;
        foreach (var line in lines)
        {
            var match = InputRegex().Match(line);
            var value = long.Parse(match.Groups[1].Value);
            var elements = match.Groups[2].Value.Split(' ').Select(long.Parse).ToArray();
            var num_bits = elements.Length - 1;

            var max_number = (long)Math.Pow(3, num_bits);

            var found = false;

            for (var current_number = 0; current_number < max_number; current_number++)
            {
                var sum = elements[0];
                var representation = LongToString(current_number, 3).PadLeft(num_bits, '0');

                for (var i = 1; i < elements.Length; i++)
                {
                    var bit = representation[i - 1] - '0';
                    if (bit == 0)
                    {
                        sum += elements[i];
                    }
                    else if (bit == 1)
                    {
                        sum *= elements[i];
                    }
                    else
                    {
                        sum = long.Parse(sum.ToString() + elements[i].ToString());
                    }
                }

                if (sum == value)
                {
                    found = true;
                    break;
                }
            }

            if (found)
            {
                total += value;
            }
        }
        return total;
    }

    [GeneratedRegex(@"(\d+): (.*)")]
    private static partial Regex InputRegex();
}
