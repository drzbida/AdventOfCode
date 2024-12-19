using System.Numerics;
using AdventOfCode.Core.Common;

namespace AdventOfCode.Solutions.Year2024;

[AdventOfCode(2024, 17)]
public partial class ChronospatialComputer : AdventOfCodeSolution<string>
{
    /*protected override string Test =>*/
    /*    """*/
    /*        Register A: 729*/
    /*        Register B: 0*/
    /*        Register C: 0*/
    /**/
    /*        Program: 0,1,5,4,3,0*/
    /*        """;*/

    /*protected override string Test =>*/
    /*    """*/
    /*        Register A: 234880*/
    /*        Register B: 0*/
    /*        Register C: 0*/
    /**/
    /*        Program: 2,4,1,7,7,5,1,7,4,6,0,3,5,5,3,0*/
    /*        """;*/

    protected override string Test =>
        """
            Register A: 2024
            Register B: 0
            Register C: 0

            Program: 0,3,5,4,3,0
            """;

    protected override string PartOne(string[] lines)
    {
        var (regA, regB, regC, program) = ParseInput(lines);
        var ip = 0;
        var output = new List<BigInteger>();

        try
        {
            while (true)
            {
                var command = Commands[program[ip]];
                command.Execute(ref regA, ref regB, ref regC, ref ip, program, output);
            }
        }
        catch (ArgumentOutOfRangeException) { }

        var outputStr = string.Join(",", output);

        return outputStr;
    }

    protected override string PartTwo(string[] lines)
    {
        var (_, regB, regC, program) = ParseInput(lines);
        var programStr = string.Join(",", program);
        for (long i = 0; i < 500_000_000_000; i++)
        {
            var output = Run(i, regB, regC, program);

            var outputStr = string.Join(",", output);
            var iInBinary = Convert.ToString(i, 2);

            Console.WriteLine($"Found : {i} | {iInBinary} -> {outputStr}");
            if (outputStr == programStr)
            {
                Console.WriteLine($"Found : {i} | {iInBinary} -> {outputStr}");
                return i.ToString();
            }
        }
        return "Not found";
    }

    private List<string> Run(BigInteger regA, BigInteger regB, BigInteger regC, List<int> program)
    {
        var ip = 0;
        var output = new List<BigInteger>();

        try
        {
            while (true)
            {
                var command = Commands[program[ip]];
                command.Execute(ref regA, ref regB, ref regC, ref ip, program, output);
            }
        }
        catch (ArgumentOutOfRangeException) { }

        return output.Select(x => x.ToString()).ToList();
    }

    private static (BigInteger A, BigInteger B, BigInteger C, List<int> program) ParseInput(
        string[] lines
    )
    {
        var registers = new Dictionary<string, BigInteger>();
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
            var value = BigInteger.Parse(parts[1]);

            registers[register] = value;
        }

        return (registers["A"], registers["B"], registers["C"], program);
    }

    private readonly Dictionary<int, Command> Commands =
        new()
        {
            { 0, new Adv() },
            { 1, new Bxl() },
            { 2, new Bst() },
            { 3, new Jnz() },
            { 4, new Bxc() },
            { 5, new Out() },
            { 6, new Bdv() },
            { 7, new Cdv() },
        };

    private abstract class Command(char type)
    {
        public char Type { get; } = type;

        public abstract void Execute(
            ref BigInteger regA,
            ref BigInteger regB,
            ref BigInteger regC,
            ref int ip,
            List<int> program,
            List<BigInteger> output
        );

        protected BigInteger GetValue(int value, BigInteger regA, BigInteger regB, BigInteger regC)
        {
            if (Type != 'C')
            {
                return value;
            }
            return value switch
            {
                var x when x <= 3 => new BigInteger(x),
                4 => regA,
                5 => regB,
                6 => regC,
                7 => throw new Exception("Should not happen"),
                _ => throw new Exception("Invalid value"),
            };
        }
    }

    private class Adv : Command
    {
        public Adv()
            : base('C') { }

        public override void Execute(
            ref BigInteger regA,
            ref BigInteger regB,
            ref BigInteger regC,
            ref int ip,
            List<int> program,
            List<BigInteger> output
        )
        {
            var literal = GetValue(program[++ip], regA, regB, regC);
            regA /= (BigInteger)Math.Pow(2, (double)literal);
            ip++;
        }
    }

    private class Bxl : Command
    {
        public Bxl()
            : base('L') { }

        public override void Execute(
            ref BigInteger regA,
            ref BigInteger regB,
            ref BigInteger regC,
            ref int ip,
            List<int> program,
            List<BigInteger> output
        )
        {
            var literal = GetValue(program[++ip], regA, regB, regC);
            regB ^= literal;
            ip++;
        }
    }

    private class Bst : Command
    {
        public Bst()
            : base('C') { }

        public override void Execute(
            ref BigInteger regA,
            ref BigInteger regB,
            ref BigInteger regC,
            ref int ip,
            List<int> program,
            List<BigInteger> output
        )
        {
            var literal = GetValue(program[++ip], regA, regB, regC);
            regB = literal % 8;
            ip++;
        }
    }

    private class Jnz : Command
    {
        public Jnz()
            : base('L') { }

        public override void Execute(
            ref BigInteger regA,
            ref BigInteger regB,
            ref BigInteger regC,
            ref int ip,
            List<int> program,
            List<BigInteger> output
        )
        {
            if (regA == 0)
            {
                ip += 2;
                return;
            }

            var literal = GetValue(program[++ip], regA, regB, regC);
            ip = (int)literal;
        }
    }

    private class Bxc : Command
    {
        public Bxc()
            : base('N') { }

        public override void Execute(
            ref BigInteger regA,
            ref BigInteger regB,
            ref BigInteger regC,
            ref int ip,
            List<int> program,
            List<BigInteger> output
        )
        {
            regB ^= regC;
            ip += 2;
        }
    }

    private class Out : Command
    {
        public Out()
            : base('C') { }

        public override void Execute(
            ref BigInteger regA,
            ref BigInteger regB,
            ref BigInteger regC,
            ref int ip,
            List<int> program,
            List<BigInteger> output
        )
        {
            var literal = GetValue(program[++ip], regA, regB, regC);
            var o = literal % 8;
            output.Add(o);
            ip++;
        }
    }

    private class Bdv : Command
    {
        public Bdv()
            : base('C') { }

        public override void Execute(
            ref BigInteger regA,
            ref BigInteger regB,
            ref BigInteger regC,
            ref int ip,
            List<int> program,
            List<BigInteger> output
        )
        {
            var literal = GetValue(program[++ip], regA, regB, regC);
            regB = regA / (BigInteger)Math.Pow(2, (double)literal);
            ip++;
        }
    }

    private class Cdv : Command
    {
        public Cdv()
            : base('C') { }

        public override void Execute(
            ref BigInteger regA,
            ref BigInteger regB,
            ref BigInteger regC,
            ref int ip,
            List<int> program,
            List<BigInteger> output
        )
        {
            var literal = GetValue(program[++ip], regA, regB, regC);
            regC = regA / (BigInteger)Math.Pow(2, (double)literal);
            ip++;
        }
    }
}
