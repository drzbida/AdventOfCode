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

    private abstract class GameObject
    {
        public int StartX { get; set; }
        public int StartY { get; set; }
        public int EndX { get; set; }
        public int EndY { get; set; }
        public char Type { get; set; }
        public abstract bool CanPush(Vector2Int dir, GameObject[,] map);
        public abstract void Push(Vector2Int dir, GameObject[,] map);
    }

    private class Wall : GameObject
    {
        public Wall(int x, int y)
        {
            StartX = x;
            StartY = y;
            EndX = x;
            EndY = y;
            Type = '#';
        }

        public override bool CanPush(Vector2Int dir, GameObject[,] map) => false;

        public override void Push(Vector2Int dir, GameObject[,] map) { }
    }

    private class Empty : GameObject
    {
        public Empty(int x, int y)
        {
            StartX = x;
            StartY = y;
            EndX = x;
            EndY = y;
            Type = '.';
        }

        public override bool CanPush(Vector2Int dir, GameObject[,] map) => true;

        public override void Push(Vector2Int dir, GameObject[,] map) { }
    }

    private class Box : GameObject
    {
        public Box(int x, int y)
        {
            StartX = x;
            StartY = y;
            EndX = x;
            EndY = y;
            Type = 'O';
        }

        public override bool CanPush(Vector2Int dir, GameObject[,] map)
        {
            var nextX = EndX + dir.X;
            var nextY = EndY + dir.Y;
            var next = map[nextX, nextY];
            return next.CanPush(dir, map);
        }

        public override void Push(Vector2Int dir, GameObject[,] map)
        {
            var nextX = EndX + dir.X;
            var nextY = EndY + dir.Y;
            var next = map[nextX, nextY];
            next.Push(dir, map);
            EndX += dir.X;
            EndY += dir.Y;
            map[EndX, EndY] = this;
            map[StartX, StartY] = new Empty(StartX, StartY);
        }
    }

    private class Player : GameObject
    {
        public Player(int x, int y)
        {
            StartX = x;
            StartY = y;
            EndX = x;
            EndY = y;
            Type = '@';
        }

        public override bool CanPush(Vector2Int dir, GameObject[,] map)
        {
            var nextX = EndX + dir.X;
            var nextY = EndY + dir.Y;
            var next = map[nextX, nextY];
            return next.CanPush(dir, map);
        }

        public override void Push(Vector2Int dir, GameObject[,] map)
        {
            var nextX = EndX + dir.X;
            var nextY = EndY + dir.Y;
            var next = map[nextX, nextY];
            next.Push(dir, map);
            EndX += dir.X;
            EndY += dir.Y;
            map[EndX, EndY] = this;
            map[StartX, StartY] = new Empty(StartX, StartY);
        }
    }

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

        MapTile[,] map = new MapTile[rawMap.Length, rawMap[0].Length];
        var player = Vector2Int.Zero;
        int n = rawMap.Length,
            m = rawMap[0].Length;

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                map[i, j] = (MapTile)rawMap[i][j];
                if (map[i, j] == MapTile.Player)
                {
                    player = new Vector2Int(i, j);
                }
            }
        }

        foreach (var movement in movements)
        {
            var dir = CharToDir(movement);
            player = Push(player, map, dir);
        }

        long result = 0;

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                if (map[i, j] != MapTile.Box)
                {
                    continue;
                }

                result += 100 * i + j;
            }
        }

        return result;
    }

    protected override long PartTwo(string[] lines)
    {
        throw new NotImplementedException();
    }

    private static Vector2Int Push(Vector2Int pos, MapTile[,] map, Vector2Int dir)
    {
        var next = pos + dir;
        if (map[next.Y, next.X] == MapTile.Edge)
        {
            return pos;
        }

        if (map[next.Y, next.X] == MapTile.Empty)
        {
            map[next.Y, next.X] = MapTile.Player;
            map[pos.Y, pos.X] = MapTile.Empty;
            return next;
        }

        var toPush = new List<Vector2Int>();

        while (map[next.Y, next.X] != MapTile.Edge && map[next.Y, next.X] != MapTile.Empty)
        {
            toPush.Add(next);
            next += dir;
        }

        if (map[next.Y, next.X] == MapTile.Edge)
        {
            return pos;
        }

        map[pos.Y, pos.X] = MapTile.Empty;

        foreach (var p in toPush)
        {
            var nextPos = p + dir;
            map[nextPos.Y, nextPos.X] = MapTile.Box;
        }

        next = pos + dir;
        map[next.Y, next.X] = MapTile.Player;
        return next;
    }

    // PART TWO - this one sucks, I won't reimplement part one to use the same logic
}
