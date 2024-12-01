namespace AdventOfCode.Core.Input;

public static class InputDownloader
{
    private static readonly string InputRoot = Path.Combine(
        Directory.GetCurrentDirectory(),
        "Inputs"
    );

    public static async Task<string> GetInputAsync(int year, int day)
    {
        string inputDirectory = Path.Combine(InputRoot, year.ToString());
        string inputFile = Path.Combine(inputDirectory, $"Day{day}.txt");

        Directory.CreateDirectory(inputDirectory);

        if (File.Exists(inputFile))
        {
            return await File.ReadAllTextAsync(inputFile);
        }

        var sessionId = Environment.GetEnvironmentVariable("AOC_SESSION");
        if (string.IsNullOrEmpty(sessionId))
        {
            throw new Exception("AOC_SESSION environment variable is not set.");
        }

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Cookie", $"session={sessionId}");
        var url = $"https://adventofcode.com/{year}/day/{day}/input";

        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Failed to fetch input for {year} Day {day}: {response.StatusCode}"
            );
        }

        var input = await response.Content.ReadAsStringAsync();

        await File.WriteAllTextAsync(inputFile, input);

        return input;
    }
}
