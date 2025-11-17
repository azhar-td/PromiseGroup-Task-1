namespace Domain.Entities
{
    public class TreeNode
    {
        public int Id { get; }
        public string Name { get; }
        public int Level { get; }
        public IReadOnlyList<TreeNode> Children { get; }

        public TreeNode(int id, string name, int level, IReadOnlyList<TreeNode> children)
        {
            Id = id;
            Name = name;
            Level = level;
            Children = children;
        }
    }
}
