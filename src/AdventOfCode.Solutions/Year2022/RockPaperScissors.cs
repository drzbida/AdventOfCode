using AdventOfCode.Core.Common;

namespace AdventOfCode.Solutions.Year2022;

[AdventOfCode(2022, 2)]
public class RockPaperScissors : AdventOfCodeSolution<int>
{
    protected override string Test =>
        """
            A Y
            B X
            C Z
            """;

    private static readonly Dictionary<char, (char weakTo, char strongTo)> _moves =
        new()
        {
            ['X'] = ('Y', 'Z'),
            ['Y'] = ('Z', 'X'),
            ['Z'] = ('X', 'Y'),
        };

    protected override int PartOne(string[] lines)
    {
        return lines
            .Select(line => (AbcToXyz(line.First()), line.Last()))
            .Sum(x => GameResultToScore(PlayGame(x.Item1, x.Item2)) + ParticipantScore(x.Item2));
    }

    protected override int PartTwo(string[] lines)
    {
        return lines
            .Select(line => (AbcToXyz(line.First()), line.Last()))
            .Sum(pair =>
            {
                var (opponent, player) = pair;
                var playerMove = GetResultToMove(opponent, CharToGameResult(player));
                return GameResultToScore(PlayGame(opponent, playerMove))
                    + ParticipantScore(playerMove);
            });
    }

    private static char AbcToXyz(char letter) => (char)(letter - 'A' + 'X');

    static int PlayGame(char opponent, char player)
    {
        var (weakTo, strongTo) = _moves[opponent];

        return player switch
        {
            var x when x == weakTo => 1,
            var x when x == strongTo => -1,
            _ => 0,
        };
    }

    static char GetResultToMove(char opponent, int result)
    {
        var (weakTo, strongTo) = _moves[opponent];
        return result switch
        {
            -1 => strongTo,
            0 => opponent,
            1 => weakTo,
            _ => throw new NotImplementedException(),
        };
    }

    static int CharToGameResult(char letter) =>
        letter switch
        {
            'X' => -1,
            'Y' => 0,
            'Z' => 1,
            _ => throw new NotImplementedException(),
        };

    static int GameResultToScore(int result) =>
        result switch
        {
            -1 => 0,
            0 => 3,
            1 => 6,
            _ => throw new NotImplementedException(),
        };

    static int ParticipantScore(char letter) => letter - 'X' + 1;
}
