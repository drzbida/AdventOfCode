using System.Reflection;
using AdventOfCode.Core.Common;

namespace AdventOfCode.ConsoleApp;

public class Program
{
    public static async Task Main(string[] args)
    {
        Assembly.Load("AdventOfCode.Solutions");
        if (args.Length > 0)
        {
            var command = args[0].ToLower();

            if (command == "all")
            {
                await RunAllSolutions();
                return;
            }

            if (
                command == "specific"
                && args.Length == 3
                && int.TryParse(args[1], out var year)
                && int.TryParse(args[2], out var day)
            )
            {
                await RunSpecificSolution(year, day);
                return;
            }

            Console.WriteLine(
                "Invalid arguments. Use 'all' to run all solutions or 'specific <year> <day>' to run a specific solution."
            );
            return;
        }

        await RunLatestSolution();
    }

    private static async Task RunLatestSolution()
    {
        var solutionTypes = GetSolutionTypes();

        var latestSolution = solutionTypes
            .Select(t => new
            {
                Type = t,
                Attribute = t.GetCustomAttribute<AdventOfCodeAttribute>(),
            })
            .OrderByDescending(t => t.Attribute?.Year)
            .ThenByDescending(t => t.Attribute?.Day)
            .FirstOrDefault();

        if (latestSolution?.Attribute == null)
        {
            Console.WriteLine("No solutions found.");
            return;
        }

        Console.WriteLine(
            $"Running latest solution: {latestSolution.Attribute.Year} Day {latestSolution.Attribute.Day}"
        );

        if (Activator.CreateInstance(latestSolution.Type) is not AdventOfCodeSolution solution)
        {
            Console.WriteLine("Failed to create an instance of the solution.");
            return;
        }

        await solution.RunAsync();
    }

    private static async Task RunSpecificSolution(int year, int day)
    {
        var solutionTypes = GetSolutionTypes();

        var solutionType = solutionTypes.FirstOrDefault(t =>
            t.GetCustomAttribute<AdventOfCodeAttribute>() is AdventOfCodeAttribute attr
            && attr.Year == year
            && attr.Day == day
        );

        if (solutionType == null)
        {
            Console.WriteLine($"No solution found for {year} Day {day}.");
            return;
        }

        if (Activator.CreateInstance(solutionType) is not AdventOfCodeSolution solution)
        {
            Console.WriteLine($"Failed to create an instance of solution for {year} Day {day}.");
            return;
        }

        Console.WriteLine($"Running solution: {year} Day {day}");
        await solution.RunAsync();
    }

    private static async Task RunAllSolutions()
    {
        var solutionTypes = GetSolutionTypes();

        var solutions = solutionTypes
            .Select(t => new
            {
                Type = t,
                Attribute = t.GetCustomAttribute<AdventOfCodeAttribute>(),
            })
            .Where(s => s.Attribute != null)
            .OrderBy(s => s.Attribute!.Year)
            .ThenBy(s => s.Attribute!.Day)
            .ToList();

        foreach (var solutionInfo in solutions)
        {
            Console.WriteLine(
                $"Running solution: {solutionInfo.Attribute!.Year} Day {solutionInfo.Attribute!.Day}"
            );

            if (Activator.CreateInstance(solutionInfo.Type) is not AdventOfCodeSolution solution)
            {
                Console.WriteLine(
                    $"Failed to create an instance of solution: {solutionInfo.Type.FullName}"
                );
                continue;
            }

            await solution.RunAsync();
        }
    }

    private static Type[] GetSolutionTypes()
    {
        return AppDomain
            .CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsSubclassOf(typeof(AdventOfCodeSolution)) && !t.IsAbstract)
            .ToArray();
    }
}
