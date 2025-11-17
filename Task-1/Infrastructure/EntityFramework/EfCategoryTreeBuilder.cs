
using Domain.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework
{
    public sealed class EfCategoryTreeBuilder : ICategoryTreeBuilder
    {
        private readonly TaskOneDbContext _db;
        public string Name => "EF+LINQ";

        public EfCategoryTreeBuilder(TaskOneDbContext db) => _db = db;

        public async Task<IReadOnlyList<TreeNode>> BuildAsync(CancellationToken ct = default)
        {
            var all = await _db.Categories
                .AsNoTracking()
                .OrderBy(c => c.ParentId)
                .ThenBy(c => c.Name)
                .ToListAsync(ct);

            // Map NULL → -1 just for dictionary key (Dictionary<TKey> requires non-null TKey)
            var byParent = all
                .GroupBy(c => c.ParentId)
                .ToDictionary(g => g.Key ?? -1, g => g.ToList());

            var roots = new List<TreeNode>();

            if (!byParent.TryGetValue(-1, out var rootCats))
                return roots;

            foreach (var root in rootCats)
                roots.Add(BuildNode(root, byParent, 0));

            return roots;
        }

        private static TreeNode BuildNode(
            Category c,
            IDictionary<int, List<Category>> byParent,
            int level)
        {
            var children = new List<TreeNode>();

            if (byParent.TryGetValue(c.Id, out var kids))
                foreach (var k in kids)
                    children.Add(BuildNode(k, byParent, level + 1));

            return new TreeNode(c.Id, c.Name, level, children);
        }

    }
}
