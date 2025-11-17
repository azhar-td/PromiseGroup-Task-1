using Domain.Abstractions;
using Domain.Entities;
using System.Diagnostics;

namespace Application.Benchmarking
{
    public class BenchmarkRunner
    {
        public static async Task<(TimeSpan elapsed, int nodeCount)> RunOnceAsync(
            ICategoryTreeBuilder builder,
            ITreePrinter printer,
            CancellationToken ct = default)
        {
            var sw = Stopwatch.StartNew();
            var tree = await builder.BuildAsync(ct);
            sw.Stop();
            var count = Count(tree);
            Console.WriteLine($"[{builder.Name}] Built {count} nodes in {sw.ElapsedMilliseconds} ms");
            await printer.PrintAsync(tree, ct);
            return (sw.Elapsed, count);
        }

        private static int Count(IReadOnlyList<TreeNode> roots)
        {
            var n = 0;
            void Walk(TreeNode node)
            {
                n++;
                foreach (var c in node.Children) Walk(c);
            }
            foreach (var r in roots) Walk(r);
            return n;
        }
    }
}
