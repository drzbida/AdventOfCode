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

        await RunSolutionAsync(latestSolution?.Type);
    }

    private static async Task RunSpecificSolution(int year, int day)
    {
        var solutionTypes = GetSolutionTypes();

        var solutionType = solutionTypes.FirstOrDefault(t =>
            t.GetCustomAttribute<AdventOfCodeAttribute>() is AdventOfCodeAttribute attr
            && attr.Year == year
            && attr.Day == day
        );

        await RunSolutionAsync(solutionType);
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
            await RunSolutionAsync(solutionInfo.Type);
        }
    }

    private static async Task<bool> RunSolutionAsync(Type? solutionType)
    {
        if (solutionType == null)
        {
            Console.WriteLine("Failed to get the solution type.");
            return false;
        }
        var createdSolution = Activator.CreateInstance(solutionType);
        if (createdSolution == null)
        {
            Console.WriteLine("Failed to create an instance of the solution.");
            return false;
        }

        dynamic solution = createdSolution;
        await solution.RunAsync();
        return true;
    }

    private static Type[] GetSolutionTypes()
    {
        return AppDomain
            .CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                t.BaseType is { IsGenericType: true }
                && t.BaseType.GetGenericTypeDefinition() == typeof(AdventOfCodeSolution<>)
            )
            .ToArray();
    }
}
