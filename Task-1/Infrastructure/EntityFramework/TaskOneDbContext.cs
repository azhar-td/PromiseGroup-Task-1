using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework
{
    public class TaskOneDbContext : DbContext
    {
        public DbSet<Category> Categories => Set<Category>();

        public TaskOneDbContext(DbContextOptions<TaskOneDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var e = modelBuilder.Entity<Category>();
            e.ToTable("Categories");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.ParentId);
        }
    }
}
