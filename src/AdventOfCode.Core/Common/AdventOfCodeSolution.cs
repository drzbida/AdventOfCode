using System.Diagnostics;
using AdventOfCode.Core.Input;
using Spectre.Console;

namespace AdventOfCode.Core.Common;

public abstract class AdventOfCodeSolution<T>
{
    public async Task RunAsync()
    {
        var attr = GetAdventOfCodeAttribute();

        var input = await InputDownloader.GetInputAsync(attr.Year, attr.Day);
        var testLines = ParseLines(Test);
        var inputLines = ParseLines(input);

        var results = string.Empty;

        var url = $"https://adventofcode.com/{attr.Year}/day/{attr.Day}";
        var headerText =
            $"[bold aqua]Name:[/] [#7f7fff][underline][link={url}]{GetType().Name}[/][/][/]\n"
            + $"[bold aqua]Year:[/] [bold yellow]{attr.Year}[/]\n"
            + $"[bold aqua]Day:[/] [bold yellow]{attr.Day}[/]\n";

        AnsiConsole
            .Live(new Panel(results).Header(headerText))
            .Start(ctx =>
            {
                var tasks = new (string Label, Func<T> Action)[]
                {
                    ("Test 1", () => PartOne(testLines)),
                    ("Part 1", () => PartOne(inputLines)),
                    ("Test 2", () => PartTwo(testLines)),
                    ("Part 2", () => PartTwo(inputLines)),
                };

                foreach (var (label, action) in tasks)
                {
                    results = AddResult(results, label, action);
                    ctx.UpdateTarget(new Panel(results).Header(headerText));
                }
            });
    }

    private AdventOfCodeAttribute GetAdventOfCodeAttribute()
    {
        if (
            Attribute.GetCustomAttribute(GetType(), typeof(AdventOfCodeAttribute))
            is not AdventOfCodeAttribute attr
        )
        {
            throw new Exception("AdventOfCodeAttribute is missing on the solution class.");
        }

        return attr;
    }

    private static string[] ParseLines(string input) =>
        input.Split("\n", StringSplitOptions.RemoveEmptyEntries);

    private static string AddResult(string currentContent, string label, Func<T> action)
    {
        var stopwatch = Stopwatch.StartNew();

        string resultMarkup;
        try
        {
            var result = action();
            stopwatch.Stop();
            var escapedResult = Markup.Escape(result?.ToString() ?? "No result").PadRight(65);

            var elapsedTime = stopwatch.ElapsedMilliseconds;
            resultMarkup =
                $"[bold yellow]{label}[/]: [green]{escapedResult}[/] ([grey]{elapsedTime}ms[/])";
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            var escapedMessage = Markup.Escape(ex.Message);
            var escapedStackTrace = Markup.Escape(ex.StackTrace ?? "No stack trace available");

            resultMarkup =
                $"[bold yellow]{label}[/]: [red]Error: {escapedMessage}[/]\n"
                + $"[bold dim]StackTrace:[/]\n[dim]{escapedStackTrace}[/]";
        }

        return string.IsNullOrEmpty(currentContent)
            ? resultMarkup
            : $"{currentContent}\n{resultMarkup}";
    }

    protected abstract T PartOne(string[] lines);
    protected abstract T PartTwo(string[] lines);
    protected abstract string Test { get; }
}
