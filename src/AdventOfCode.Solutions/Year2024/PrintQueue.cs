using AdventOfCode.Core.Common;
using AdventOfCode.Core.Utils;

namespace AdventOfCode.Solutions.Year2024;

[AdventOfCode(2024, 5)]
public partial class PrintQueue : AdventOfCodeSolution<int>
{
    protected override string Test =>
        """
            47|53
            97|13
            97|61
            97|47
            75|29
            61|13
            75|53
            29|13
            97|29
            53|29
            61|53
            97|53
            61|29
            47|13
            75|47
            97|75
            47|61
            75|61
            47|29
            75|13
            53|13

            75,47,61,53,29
            97,61,53,29,13
            75,29,13
            75,97,47,61,53
            61,13,29
            97,13,75,29,47
            """;

    protected override int PartOne(string[] lines)
    {
        var chunked = lines.PartitionBy(string.IsNullOrWhiteSpace).Take(2).ToList();

        var orderingA = CreateOrderingMap(chunked[0]);
        var priorityQueue = CreatePriorityQueue(orderingA);

        return chunked[1]
            .Select(line => line.Split(',').Select(int.Parse).ToList())
            .Where(values => IsOrderingValid(values, orderingA, priorityQueue))
            .Select(values => values[values.Count / 2])
            .Sum();
    }

    protected override int PartTwo(string[] lines)
    {
        var chunked = lines.PartitionBy(string.IsNullOrWhiteSpace).Take(2).ToList();

        var orderingA = CreateOrderingMap(chunked[0]);
        var priorityQueue = CreatePriorityQueue(orderingA);

        return chunked[1]
            .Select(line => line.Split(',').Select(int.Parse).ToList())
            .Where(values => !IsOrderingValid(values, orderingA, priorityQueue))
            .Sum(values =>
            {
                priorityQueue.Clear();
                values.ForEach(value => priorityQueue.Enqueue(value, value));
                while (priorityQueue.Count > values.Count / 2 + 1)
                {
                    priorityQueue.Dequeue();
                }
                return priorityQueue.Dequeue();
            });
    }

    private static Dictionary<int, List<int>> CreateOrderingMap(List<string> lines)
    {
        var orderingMap = new Dictionary<int, List<int>>();

        foreach (var line in lines)
        {
            var parts = line.Split('|');
            var a = int.Parse(parts[0]);
            var b = int.Parse(parts[1]);

            if (!orderingMap.TryGetValue(a, out List<int>? value))
            {
                value = [];
                orderingMap[a] = value;
            }

            value.Add(b);
        }

        return orderingMap;
    }

    private static PriorityQueue<int, int> CreatePriorityQueue(
        Dictionary<int, List<int>> orderingA
    ) =>
        new(
            Comparer<int>.Create(
                (a, b) =>
                    orderingA.TryGetValue(a, out List<int>? listA) && listA.Contains(b) ? -1 : 1
            )
        );

    private static bool IsOrderingValid(
        List<int> values,
        Dictionary<int, List<int>> orderingA,
        PriorityQueue<int, int> priorityQueue
    )
    {
        priorityQueue.Clear();
        var impactedValues = values.Where(orderingA.ContainsKey).ToList();
        impactedValues.ForEach(value => priorityQueue.Enqueue(value, value));
        return impactedValues.SequenceEqual(
            Enumerable.Range(0, priorityQueue.Count).Select(_ => priorityQueue.Dequeue())
        );
    }
}
