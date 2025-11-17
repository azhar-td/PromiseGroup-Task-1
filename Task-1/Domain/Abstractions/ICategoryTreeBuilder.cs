using Domain.Entities;

namespace Domain.Abstractions
{
    public interface ICategoryTreeBuilder
    {
        Task<IReadOnlyList<TreeNode>> BuildAsync(CancellationToken ct = default);

        // "EF+LINQ" | "StoredProc"
        string Name { get; } 
    }
}
