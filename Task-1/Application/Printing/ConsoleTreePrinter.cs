using Domain.Abstractions;
using Domain.Entities;

namespace Application.Printing
{
    public class ConsoleTreePrinter : ITreePrinter
    {
        public Task PrintAsync(IReadOnlyList<TreeNode> roots, CancellationToken ct = default)
        {
            foreach (var r in roots)
                Print(r);
            return Task.CompletedTask;
        }

        private static void Print(TreeNode node)
        {
            Console.WriteLine($"{new string(' ', node.Level * 2)}- {node.Name}");
            foreach (var c in node.Children)
                Print(c);
        }
    }
}
