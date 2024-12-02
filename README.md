My solutions for advent of code. Input files are automatically downloaded and cached. By default, the latest day from the latest year is automatically ran. If you want to use this as a template, just delete all files under AdventOfCode.Solutions and add your own.

## Usage

Grab your session cookie from advent of code and set it as an environment variable

Linux
```bash
export AOC_SESSION=YOUR_SESSION_COOKIE
```

Powershell
```powershell
$env:AOC_SESSION="YOUR_SESSION_COOKIE"
```

Cmd
```cmd
set AOC_SESSION=YOUR_SESSION_COOKIE
```

Run the app
```bash
dotnet run --project .\src\AdventOfCode.ConsoleApp\
```

## Implement your own

All you need to do to create a solution for a specific day is to extend the `AdventOfCodeSolution` class and use the correct `AdventOfCode` attributes

```csharp
using AdventOfCode.Core.Common;

namespace AdventOfCode.Solutions.Year2024;

[AdventOfCode(2024, 1)]
public class Day1 : AdventOfCodeSolution<int>
{
    protected override string Test =>
        """
            3   4
            4   3
            2   5
            1   3
            3   9
            3   3
            """;

    protected override int PartOne(string[] lines)
    {
        Thread.Sleep(2000);
        return 1;
    }

    protected override int PartTwo(string[] lines)
    {
        Thread.Sleep(1000);
        throw new NotImplementedException();
    }
}
```

https://github.com/user-attachments/assets/de4431bb-6cba-421f-82d4-d36a15b6cdae



