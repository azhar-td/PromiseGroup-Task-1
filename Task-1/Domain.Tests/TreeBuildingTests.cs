using Domain.Entities;
using FluentAssertions;

namespace Domain.Tests
{
    public class TreeBuildingTests
    {
        [Fact]
        public void TreeNode_CreatesExpectedShape()
        {
            var leaf = new TreeNode(5, "Leaf", 3, new List<TreeNode>());
            var child = new TreeNode(2, "Child", 1, new List<TreeNode> { leaf });
            var root = new TreeNode(1, "Root", 0, new List<TreeNode> { child });

            root.Level.Should().Be(0);
            root.Children.Should().HaveCount(1);
            root.Children[0].Children[0].Name.Should().Be("Leaf");
        }
    }
}
