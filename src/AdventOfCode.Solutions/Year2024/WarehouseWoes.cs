using AdventOfCode.Core.Common;
using AdventOfCode.Core.Utils;

namespace AdventOfCode.Solutions.Year2024;

[AdventOfCode(2024, 15)]
public partial class WarehouseWoes : AdventOfCodeSolution<long>
{
    protected override string Test =>
        """
            ##########
            #..O..O.O#
            #......O.#
            #.OO..O.O#
            #..O@..O.#
            #O#..O...#
            #O..O..O.#
            #.OO.O.OO#
            #....O...#
            ##########

            <vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^
            vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v
            ><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<
            <<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^
            ^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><
            ^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^
            >^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^
            <><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>
            ^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>
            v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^
            """;

    // VERY overkill for what it was needed, did it for fun
    // as i needed to refactor the code to support the second part anyway
    // supports infinite width and height for the objects

    private abstract class GameObject(Vector2Int origin, int width, int height, char[] display)
    {
        public Vector2Int Origin { get; set; } = origin;
        public int Width { get; set; } = width;
        public int Height { get; set; } = height;
        public char[] Display { get; set; } = display;
        public abstract bool CanPush(Vector2Int dir, GameObject[,] map);
        public abstract void Push(Vector2Int dir, GameObject[,] map);

        public override string ToString() => string.Join("", Display);
    }

    private abstract class Immovable(Vector2Int origin, int width, int height, char[] display)
        : GameObject(origin, width, height, display)
    {
        public override bool CanPush(Vector2Int dir, GameObject[,] map) => false;

        public override void Push(Vector2Int dir, GameObject[,] map) { }
    }

    private abstract class Pushable(Vector2Int origin, int width, int height, char[] display)
        : GameObject(origin, width, height, display)
    {
        private List<Vector2Int> GetObjectEdge(Vector2Int dir)
        {
            var posToCheck = new List<Vector2Int>();

            if (dir == Vector2Int.Up)
            {
                for (int x = 0; x < Width; x++)
                {
                    posToCheck.Add(Origin + new Vector2Int(x, 0));
                }
            }
            else if (dir == Vector2Int.Down)
            {
                for (int x = 0; x < Width; x++)
                {
                    posToCheck.Add(Origin + new Vector2Int(x, Height - 1));
                }
            }
            else if (dir == Vector2Int.Left)
            {
                for (int y = 0; y < Height; y++)
                {
                    posToCheck.Add(Origin + new Vector2Int(0, y));
                }
            }
            else if (dir == Vector2Int.Right)
            {
                for (int y = 0; y < Height; y++)
                {
                    posToCheck.Add(Origin + new Vector2Int(Width - 1, y));
                }
            }

            return posToCheck;
        }

        public override bool CanPush(Vector2Int dir, GameObject[,] map)
        {
            var posToCheck = GetObjectEdge(dir);
            return posToCheck.All(pos =>
            {
                var nextPos = pos + dir;
                var next = map[nextPos.Y, nextPos.X];
                return next.CanPush(dir, map);
            });
        }

        public override void Push(Vector2Int dir, GameObject[,] map)
        {
            var posToCheck = GetObjectEdge(dir);
            foreach (var pos in posToCheck)
            {
                var nextPos = pos + dir;
                var next = map[nextPos.Y, nextPos.X];
                next.Push(dir, map);
            }

            for (var y = Origin.Y; y < Origin.Y + Height; y++)
            {
                for (var x = Origin.X; x < Origin.X + Width; x++)
                {
                    map[y, x] = new Empty(new Vector2Int(x, y));
                }
            }

            Origin += dir;

            for (var y = Origin.Y; y < Origin.Y + Height; y++)
            {
                for (var x = Origin.X; x < Origin.X + Width; x++)
                {
                    map[y, x] = this;
                }
            }
        }
    }

    private class Wall(Vector2Int origin) : Immovable(origin, 1, 1, ['#']) { }

    private class Empty(Vector2Int origin) : GameObject(origin, 1, 1, ['.'])
    {
        public override bool CanPush(Vector2Int dir, GameObject[,] map) => true;

        public override void Push(Vector2Int dir, GameObject[,] map) { }
    }

    private class Box(Vector2Int origin) : Pushable(origin, 1, 1, ['O']) { }

    private class DoubleBox(Vector2Int origin) : Pushable(origin, 2, 1, ['[', ']']) { }

    private class Player(Vector2Int origin) : Pushable(origin, 1, 1, ['@']) { }

    private static Vector2Int CharToDir(char c) =>
        c switch
        {
            '^' => Vector2Int.Up,
            'v' => Vector2Int.Down,
            '<' => Vector2Int.Left,
            '>' => Vector2Int.Right,
            _ => throw new InvalidOperationException(),
        };

    protected override long PartOne(string[] lines)
    {
        var chunked = lines.PartitionBy(string.IsNullOrWhiteSpace);
        var rawMap = chunked.First().Select(x => x.ToCharArray()).ToArray();
        var movements = string.Join("", chunked.Skip(1).SelectMany(x => x)).ToCharArray();

        int n = rawMap.Length,
            m = rawMap[0].Length;

        var map = new GameObject[n, m];
        Player player = new(Vector2Int.Zero);

        for (int y = 0; y < n; y++)
        {
            for (int x = 0; x < m; x++)
            {
                var c = rawMap[y][x];
                var origin = new Vector2Int(x, y);
                map[y, x] = c switch
                {
                    '#' => new Wall(origin),
                    '.' => new Empty(origin),
                    'O' => new Box(origin),
                    '@' => new Player(origin),
                    _ => throw new InvalidOperationException(),
                };

                if (map[y, x] is Player player1)
                {
                    player = player1;
                }
            }
        }

        return Play(player, map, movements);
    }

    protected override long PartTwo(string[] lines)
    {
        var chunked = lines.PartitionBy(string.IsNullOrWhiteSpace);
        var rawMap = chunked.First().Select(x => x.ToCharArray()).ToArray();
        var movements = string.Join("", chunked.Skip(1).SelectMany(x => x)).ToCharArray();

        int n = rawMap.Length,
            origM = rawMap[0].Length;

        var map = new GameObject[n, (origM * 2)];
        Player player = new(Vector2Int.Zero);

        for (int y = 0; y < n; y++)
        {
            var newX = 0;
            for (int x = 0; x < origM; x++)
            {
                var c = rawMap[y][x];
                var origin = new Vector2Int(newX, y);
                var nextOrigin = origin + Vector2Int.Right;

                if (c == '@')
                {
                    player = new Player(origin);
                    map[y, newX] = player;
                    map[y, newX + 1] = new Empty(nextOrigin);
                }
                else if (c == 'O')
                {
                    var doubleBox = new DoubleBox(origin);
                    map[y, newX] = doubleBox;
                    map[y, newX + 1] = doubleBox;
                }
                else if (c == '#')
                {
                    map[y, newX] = new Wall(origin);
                    map[y, newX + 1] = new Wall(nextOrigin);
                }
                else if (c == '.')
                {
                    map[y, newX] = new Empty(origin);
                    map[y, newX + 1] = new Empty(nextOrigin);
                }

                newX += 2;
            }
        }

        return Play(player, map, movements);
    }

    private static long Play(Player player, GameObject[,] map, char[] movements)
    {
        foreach (var movement in movements)
        {
            var dir = CharToDir(movement);
            if (player.CanPush(dir, map))
            {
                player.Push(dir, map);
            }
        }

        long result = 0;

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x].Origin != new Vector2Int(x, y))
                {
                    continue;
                }
                if (map[y, x] is Box or DoubleBox)
                {
                    result += y * 100 + x;
                }
            }
        }

        return result;
    }
}
