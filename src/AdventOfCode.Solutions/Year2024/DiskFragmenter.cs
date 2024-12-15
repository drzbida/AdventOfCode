using AdventOfCode.Core.Common;

namespace AdventOfCode.Solutions.Year2024;

[AdventOfCode(2024, 9)]
public class DiskFragmenter : AdventOfCodeSolution<long>
{
    protected override string Test =>
        """
            2333133121414131402
            """;

    protected override long PartOne(string[] lines)
    {
        var input = lines[0].ToCharArray();
        List<int> buffer = [];

        var id = 0;
        for (var i = 0; i < input.Length; i++)
        {
            if (i % 2 == 0)
            {
                buffer.AddRange(Enumerable.Repeat(id, input[i] - '0'));
                id++;
            }
            else
            {
                buffer.AddRange(Enumerable.Repeat(-1, input[i] - '0'));
            }
        }

        var head = 0;
        var tail = buffer.Count - 1;
        long result = 0;

        while (head <= tail)
        {
            if (buffer[head] != -1)
            {
                result += buffer[head] * head;
                head++;
                continue;
            }

            if (buffer[tail] == -1)
            {
                tail--;
                continue;
            }

            buffer[head] = buffer[tail];
            buffer[tail] = -1;
            result += buffer[head] * head;
            head++;
            tail--;
        }

        return result;
    }

    protected override long PartTwo(string[] lines)
    {
        var input = lines[0].AsSpan();

        var blocks = new Dictionary<int, PriorityQueue<MemoryBlock, MemoryBlock>>();

        for (int i = 1; i <= 9; i++)
        {
            blocks.Add(i, CreateBlockQueue());
        }

        var files = new List<File>();
        int maxId = 0,
            totalBits = 0;

        for (int i = 0; i < input.Length; i++)
        {
            var size = input[i] - '0';
            if (i % 2 == 0)
            {
                files.Add(
                    new File
                    {
                        Id = maxId++,
                        Size = size,
                        OriginalStart = totalBits,
                    }
                );
            }
            else
            {
                if (size == 0)
                    continue;
                var queue = blocks[size];
                var memoryBlock = new MemoryBlock { Start = totalBits, Size = size };
                queue.Enqueue(memoryBlock, memoryBlock);
            }
            totalBits += size;
        }

        long result = 0;
        for (var i = files.Count - 1; i >= 0; i--)
        {
            var file = files[i];

            MemoryBlock? bestBlock = null;

            for (var blockSize = file.Size; blockSize <= 9; blockSize++)
            {
                var sizeQueue = blocks[blockSize];
                if (sizeQueue.Count == 0)
                {
                    continue;
                }
                var peekBlock = sizeQueue.Peek();
                if (
                    peekBlock.Start < file.OriginalStart
                    && (bestBlock == null || (peekBlock.Start < bestBlock?.Start))
                )
                {
                    bestBlock = peekBlock;
                }
            }

            if (bestBlock == null)
            {
                result += CalculateSum(file.OriginalStart, file.Size, file.Id);
                continue;
            }

            var queue = blocks[bestBlock!.Value.Size];
            var block = queue.Dequeue();

            result += CalculateSum(block.Start, file.Size, file.Id);
            var newSize = block.Size - file.Size;
            if (newSize <= 0)
            {
                continue;
            }

            var newBlock = new MemoryBlock { Start = block.Start + file.Size, Size = newSize };
            blocks[newSize].Enqueue(newBlock, newBlock);
        }

        return result;
    }

    private static PriorityQueue<MemoryBlock, MemoryBlock> CreateBlockQueue() =>
        new(Comparer<MemoryBlock>.Create((a, b) => a.Start.CompareTo(b.Start)));

    private static long CalculateSum(int start, int size, int id)
    {
        long n = start + size - 1;
        return ((n * (n + 1) / 2) - ((start - 1) * (long)start / 2)) * id;
    }

    struct MemoryBlock
    {
        public int Start { get; set; }
        public int Size { get; set; }
    }

    struct File
    {
        public int Id { get; set; }
        public int Size { get; set; }
        public int OriginalStart { get; set; }
    }
}
