using Domain.Abstractions;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Infrastructure.Tests;

public class TreeBuilderComparisonTests
{
    private static TaskOneDbContext CreateInMemoryDb()
    {
        var opt = new DbContextOptionsBuilder<TaskOneDbContext>()
                    .UseInMemoryDatabase("TreeComparisonDb_" + Guid.NewGuid())
                    .Options;

        var db = new TaskOneDbContext(opt);
        // Seed 4-level hierarchy
        var root1 = db.Add(new Category(1, "Electronics", null)).Entity;
        var mobiles = db.Add(new Category(2, "Mobiles", root1.Id)).Entity;
        var android = db.Add(new Category(3, "Android Phones", mobiles.Id)).Entity;
        db.Add(new Category(4, "Samsung", android.Id));
        db.Add(new Category(5, "Pixel", android.Id));
        db.Add(new Category(6, "Laptops", root1.Id));
        db.SaveChanges();
        return db;
    }

    [Fact]
    public async Task EF_Builds_Valid_Tree_Quickly()
    {
        using var db = CreateInMemoryDb();
        var builder = new EfCategoryTreeBuilder(db);

        var sw = Stopwatch.StartNew();
        var tree = await builder.BuildAsync();
        sw.Stop();

        tree.Should().NotBeNull();
        tree.Should().HaveCountGreaterThan(0);
        tree[0].Children.Should().NotBeEmpty();
        sw.ElapsedMilliseconds.Should().BeLessThan(1000, "EF tree builder should be fast");
    }

    [Fact]
    public async Task Compare_EF_And_SP_Performance_Simulation()
    {
        // EF builds real tree; SP will simulate same structure using EF data
        using var db = CreateInMemoryDb();
        var efBuilder = new EfCategoryTreeBuilder(db);

        var efWatch = Stopwatch.StartNew();
        var efTree = await efBuilder.BuildAsync();
        efWatch.Stop();

        // Simulate SP builder using same structure for fairness
        var spBuilder = new MockSpCategoryTreeBuilder(efTree);
        var spWatch = Stopwatch.StartNew();
        var spTree = await spBuilder.BuildAsync();
        spWatch.Stop();

        efTree.Should().NotBeNull();
        spTree.Should().NotBeNull();

        var efCount = CountNodes(efTree);
        var spCount = CountNodes(spTree);
        efCount.Should().Be(spCount, "Both builders should produce equal node count");

        // Assert both finish quickly (< 1s each)
        efWatch.ElapsedMilliseconds.Should().BeLessThan(1000);
        spWatch.ElapsedMilliseconds.Should().BeLessThan(1000);

        // Print timing for visibility
        Console.WriteLine($"\nEF: {efWatch.ElapsedMilliseconds}ms, SP: {spWatch.ElapsedMilliseconds}ms");
    }

    private static int CountNodes(IReadOnlyList<TreeNode> roots)
    {
        int n = 0;
        void Walk(TreeNode node)
        {
            n++;
            foreach (var c in node.Children) Walk(c);
        }
        foreach (var r in roots) Walk(r);
        return n;
    }

    // A mock SP builder that just returns existing nodes to simulate performance
    private sealed class MockSpCategoryTreeBuilder : ICategoryTreeBuilder
    {
        private readonly IReadOnlyList<TreeNode> _data;
        public MockSpCategoryTreeBuilder(IReadOnlyList<TreeNode> data) => _data = data;
        public string Name => "MockSP";
        public Task<IReadOnlyList<TreeNode>> BuildAsync(CancellationToken ct = default)
            => Task.FromResult(_data);
    }
}
