using AdventOfCode.Core.Common;

namespace AdventOfCode.Solutions.Year2024;

[AdventOfCode(2024, 17)]
public partial class ChronospatialComputer : AdventOfCodeSolution<string>
{
    protected override string Test =>
        """
            Register A: 729
            Register B: 0
            Register C: 0

            Program: 0,1,5,4,3,0
            """;

    protected override string PartOne(string[] lines)
    {
        var (regA, regB, regC, program) = ParseInput(lines);
        return string.Join(",", Run(regA, regB, regC, program));
    }

    protected override string PartTwo(string[] lines)
    {
        // Not sure if there is a smarter way to solve this
        // I've printed a lot of outputs from i=1..N with the binary
        // represeentation of i and it seems to be a pattern
        // I can see at the begggining that values of i between
        // 2^0 - 2^1 have a single outputs
        // 2^1 - 2^2 have two outpus
        // 2^2 - 2^3 have three outputs
        // At some point this stops being true (I think they start repeeating)
        // but I think we can get closer to the answer by finding the appropriate
        // bits for each value in the output. It seems that 3 bits at the begggining
        // are used for the last 1 output, and so on

        var (_, regB, regC, program) = ParseInput(lines);
        var target = new List<int>(program);
        target.Reverse();

        return FindRegA(0, 0, target, program, regB, regC).ToString();
    }

    private static long FindRegA(
        long value,
        int depth,
        List<int> target,
        List<int> program,
        long regB,
        long regC
    )
    {
        if (depth == target.Count)
        {
            return value;
        }

        for (var i = 0; i < 8; i++)
        {
            var output = Run(value * 8 + i, regB, regC, program);

            if (output.First() == target[depth])
            {
                var result = FindRegA(value * 8 + i, depth + 1, target, program, regB, regC);
                if (result != -1)
                {
                    return result;
                }
            }
        }

        return -1;
    }

    private static List<int> Run(long regA, long regB, long regC, List<int> program)
    {
        var ip = 0;
        var output = new List<long>();

        try
        {
            while (true)
            {
                Execute(program[ip], ref regA, ref regB, ref regC, ref ip, program, output);
            }
        }
        catch (ArgumentOutOfRangeException) { }

        return output.Select(x => (int)x).ToList();
    }

    private static (long A, long B, long C, List<int> program) ParseInput(string[] lines)
    {
        var registers = new Dictionary<string, long>();
        var program = new List<int>();

        foreach (var line in lines)
        {
            if (line.StartsWith("Program"))
            {
                program = line.Split(": ")[1].Split(",").Select(int.Parse).ToList();
                continue;
            }
            if (!line.StartsWith("Register"))
            {
                continue;
            }
            var parts = line.Split(": ");
            var register = parts[0].Split(" ")[1];
            var value = long.Parse(parts[1]);

            registers[register] = value;
        }

        return (registers["A"], registers["B"], registers["C"], program);
    }

    private static long GetValue(int value, long regA, long regB, long regC, char type)
    {
        if (type != 'C')
        {
            return value;
        }
        return value switch
        {
            var x when x <= 3 => x,
            4 => regA,
            5 => regB,
            6 => regC,
            7 => throw new Exception("Should not happen"),
            _ => throw new Exception("Invalid value"),
        };
    }

    private static void Execute(
        int opcode,
        ref long regA,
        ref long regB,
        ref long regC,
        ref int ip,
        List<int> program,
        List<long> output
    )
    {
        switch (opcode)
        {
            case 0:
            {
                var literal = GetValue(program[++ip], regA, regB, regC, 'C');
                regA /= (long)Math.Pow(2, literal);
                ip++;
                break;
            }
            case 1:
            {
                var literal = GetValue(program[++ip], regA, regB, regC, 'L');
                regB ^= literal;
                ip++;
                break;
            }
            case 2:
            {
                var literal = GetValue(program[++ip], regA, regB, regC, 'C');
                regB = literal % 8;
                ip++;
                break;
            }
            case 3:
            {
                if (regA == 0)
                {
                    ip += 2;
                    return;
                }

                var literal = GetValue(program[++ip], regA, regB, regC, 'L');
                ip = (int)literal;
                break;
            }
            case 4:
            {
                regB ^= regC;
                ip += 2;
                break;
            }
            case 5:
            {
                var literal = GetValue(program[++ip], regA, regB, regC, 'C');
                var o = literal % 8;
                output.Add(o);
                ip++;
                break;
            }
            case 6:
            {
                var literal = GetValue(program[++ip], regA, regB, regC, 'C');
                regB = regA / (long)Math.Pow(2, literal);
                ip++;
                break;
            }
            case 7:
            {
                var literal = GetValue(program[++ip], regA, regB, regC, 'C');
                regC = regA / (long)Math.Pow(2, literal);
                ip++;
                break;
            }
        }
    }
}
