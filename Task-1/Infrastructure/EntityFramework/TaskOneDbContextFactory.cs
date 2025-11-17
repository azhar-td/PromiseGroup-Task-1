using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.EntityFramework
{
    public class TaskOneDbContextFactory : IDesignTimeDbContextFactory<TaskOneDbContext>
    {
        public TaskOneDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            var optionsBuilder = new DbContextOptionsBuilder<TaskOneDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new TaskOneDbContext(optionsBuilder.Options);
        }
    }
}
