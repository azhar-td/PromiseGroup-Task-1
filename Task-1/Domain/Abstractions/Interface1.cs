using Domain.Entities;

namespace Domain.Abstractions
{
    public interface ITreePrinter
    {
        Task PrintAsync(IReadOnlyList<TreeNode> roots, CancellationToken ct = default);
    }
}
