using System.Diagnostics;
using AdventOfCode.Core.Input;
using Spectre.Console;

namespace AdventOfCode.Core.Common;

public abstract class AdventOfCodeSolution
{
    public async Task RunAsync()
    {
        var attr = GetAdventOfCodeAttribute();

        var input = await InputDownloader.GetInputAsync(attr.Year, attr.Day);
        var testLines = ParseLines(Test);
        var inputLines = ParseLines(input);

        var results = string.Empty;

        var headerText =
            $"[bold aqua]Year:[/] [bold yellow]{attr.Year}[/]  [bold aqua]Day:[/] [bold yellow]{attr.Day}[/]\n";
        AnsiConsole
            .Live(new Panel(results).Header(headerText).Expand())
            .Start(ctx =>
            {
                var tasks = new (string Label, Func<string> Action)[]
                {
                    ("Test Part 1", () => PartOne(testLines)),
                    ("Part 1", () => PartOne(inputLines)),
                    ("Test Part 2", () => PartTwo(testLines)),
                    ("Part 2", () => PartTwo(inputLines)),
                };

                foreach (var (label, action) in tasks)
                {
                    results = AddResult(results, label, action);
                    ctx.UpdateTarget(new Panel(results).Header(headerText).Expand());
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

    private static string AddResult(string currentContent, string label, Func<string> action)
    {
        label = label.PadRight(20);
        var stopwatch = Stopwatch.StartNew();

        string resultMarkup;
        try
        {
            var result = action();
            var escapedResult = Markup.Escape(result);
            stopwatch.Stop();

            var elapsedTime = stopwatch.ElapsedMilliseconds;
            resultMarkup =
                $"[bold yellow]{label}[/]: [green]{escapedResult}[/] ([grey]{elapsedTime}ms[/])";
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            var elapsedTime = stopwatch.ElapsedMilliseconds;
            var escapedMessage = Markup.Escape(ex.Message);
            var escapedStackTrace = Markup.Escape(ex.StackTrace ?? "No stack trace available");

            resultMarkup =
                $"[bold yellow]{label}[/]: [red]Error: {escapedMessage}[/] ([grey]{elapsedTime}ms[/])\n"
                + $"[bold dim]StackTrace:[/]\n[dim]{escapedStackTrace}[/]";
        }

        return string.IsNullOrEmpty(currentContent)
            ? resultMarkup
            : $"{currentContent}\n{resultMarkup}";
    }

    protected abstract string PartOne(string[] lines);
    protected abstract string PartTwo(string[] lines);
    protected abstract string Test { get; }
}
