namespace AdventOfCode.Core.Utils;

public static class EnumerableExtensions
{
    public static IEnumerable<List<T>> PartitionBy<T>(
        this IEnumerable<T> source,
        Func<T, bool> predicate
    )
    {
        List<T> partition = [];
        foreach (var item in source)
        {
            if (predicate(item) && partition.Count > 0)
            {
                yield return partition;
                partition = [];
            }
            else
            {
                partition.Add(item);
            }
        }
        if (partition.Count > 0)
        {
            yield return partition;
        }
    }
}
