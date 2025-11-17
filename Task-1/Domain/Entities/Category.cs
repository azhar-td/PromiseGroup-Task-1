
namespace Domain.Entities
{
    public class Category
    {
        public int Id { get; }
        public string Name { get; }
        public int? ParentId { get; }

        public Category(int id, string name, int? parentId)
        {
            Id = id;
            Name = name;
            ParentId = parentId;
        }
    }
}
