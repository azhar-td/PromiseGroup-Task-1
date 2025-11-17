using Application.Printing;
using Domain.Entities;
using FluentAssertions;

namespace Domain.Tests
{
    public class ConsolePrinterTests
    {
        [Fact]
        public async Task PrintsIndentedTree()
        {
            var roots = new[]
            {
                new TreeNode(1, "Root", 0, new List<TreeNode>{
                    new TreeNode(2, "Child", 1, new List<TreeNode>{
                        new TreeNode(3, "Grandchild", 2, new List<TreeNode>())
                    })
                })
            };

            var sw = new StringWriter();
            Console.SetOut(sw);

            var printer = new ConsoleTreePrinter();
            await printer.PrintAsync(roots);

            var output = sw.ToString().Trim();
            output.Should().Contain("- Root")
                  .And.Contain("  - Child")
                  .And.Contain("    - Grandchild");
        }
    }
}
