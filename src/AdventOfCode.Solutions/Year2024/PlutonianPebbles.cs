using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

    // Human solution
    /* Original solution
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
    */

    private static readonly long[] Pow10 =
    [
        1L,
        10L,
        100L,
        1_000L,
        10_000L,
        100_000L,
        1_000_000L,
        10_000_000L,
        100_000_000L,
        1_000_000_000L,
    ];

    public static long Solve(string input, int blinks)
    {
        // this shaves off around 10ms with Release
        // don't think it's possible to get this in microseconds range with my current setup for Part 2
        // or some crazy custom dictionary implementation
        // for future self: if interested try to use a smarter caching mechanism, basic DP seems slower
        var currentDict = new Dictionary<long, long>(512);

        var parts = input.Split(' ');
        foreach (var part in parts)
        {
            long number = long.Parse(part);
            ref long countRef = ref GetValueRefOrAdd(currentDict, number);
            countRef += 1;
        }

        for (int i = 0; i < blinks; i++)
        {
            var nextDict = new Dictionary<long, long>(currentDict.Count * 2);

            foreach (var entry in currentDict)
            {
                long key = entry.Key;
                long value = entry.Value;

                if (key == 0)
                {
                    ref long refVal = ref GetValueRefOrAdd(nextDict, 1);
                    refVal += value;
                    continue;
                }

                int digits = CountDigits(key);

                if ((digits & 1) == 0)
                {
                    int half = digits >> 1;
                    long p = Pow10[half];

                    long firstHalf = key / p;
                    long secondHalf = key % p;

                    ref long firstRef = ref GetValueRefOrAdd(nextDict, firstHalf);
                    firstRef += value;

                    ref long secondRef = ref GetValueRefOrAdd(nextDict, secondHalf);
                    secondRef += value;
                }
                else
                {
                    ref long refVal = ref GetValueRefOrAdd(nextDict, key * 2024);
                    refVal += value;
                }
            }

            currentDict = nextDict;
        }

        return currentDict.Values.Sum();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CountDigits(long x)
    {
        if (x < 10L)
            return 1;
        if (x < 100L)
            return 2;
        if (x < 1000L)
            return 3;
        if (x < 10000L)
            return 4;
        if (x < 100000L)
            return 5;
        if (x < 1000000L)
            return 6;
        if (x < 10000000L)
            return 7;
        if (x < 100000000L)
            return 8;
        if (x < 1000000000L)
            return 9;
        if (x < 10000000000L)
            return 10;
        if (x < 100000000000L)
            return 11;
        if (x < 1000000000000L)
            return 12;
        if (x < 10000000000000L)
            return 13;
        if (x < 100000000000000L)
            return 14;
        if (x < 1000000000000000L)
            return 15;
        if (x < 10000000000000000L)
            return 16;
        if (x < 100000000000000000L)
            return 17;
        if (x < 1000000000000000000L)
            return 18;
        return 19;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ref long GetValueRefOrAdd(Dictionary<long, long> dict, long key)
    {
        ref long valRef = ref CollectionsMarshal.GetValueRefOrNullRef(dict, key);
        if (!Unsafe.IsNullRef(ref valRef))
        {
            return ref valRef;
        }

        return ref CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out _);
    }
}
