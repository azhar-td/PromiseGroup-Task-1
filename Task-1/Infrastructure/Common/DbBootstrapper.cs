using Domain.Entities;
using Infrastructure.EntityFramework;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common
{
    public static class DbBootstrapper
    {
        public static async Task EnsureCreatedAndSeededAsync(TaskOneDbContext db, string connectionString, CancellationToken ct = default)
        {
            await db.Database.MigrateAsync(ct); // if you prefer migrations
                                                // or: await db.Database.EnsureCreatedAsync(ct);

            // seed if empty
            if (!await db.Categories.AnyAsync(ct))
            {
                var ids = new Dictionary<string, int>();

                int Add(string name, int? parent)
                {
                    var c = new Category(0, name, parent);
                    db.Add(c);
                    db.SaveChanges();
                    ids[name] = c.Id;
                    return c.Id;
                }

                var electronics = Add("Electronics", null);
                var home = Add("Home & Kitchen", null);

                var mobiles = Add("Mobiles", electronics);
                var laptops = Add("Laptops", electronics);
                var furniture = Add("Furniture", home);

                var android = Add("Android Phones", mobiles);
                var ios = Add("iOS Phones", mobiles);
                var gaming = Add("Gaming Laptops", laptops);
                var chairs = Add("Office Chairs", furniture);

                Add("Samsung", android);
                Add("Google Pixel", android);
                Add("iPhone Pro", ios);
                Add("Ergonomic Chair", chairs);
            }

            // create/replace SP
            const string sp = """
                    CREATE OR ALTER PROCEDURE dbo.GetCategoryTree
                    AS
                    BEGIN
                        SET NOCOUNT ON;
                        ;WITH CatCTE AS
                        (
                          SELECT Id, Name, ParentId, 0 AS Level
                          FROM dbo.Categories WHERE ParentId IS NULL
                          UNION ALL
                          SELECT c.Id, c.Name, c.ParentId, cte.Level + 1
                          FROM dbo.Categories c
                          INNER JOIN CatCTE cte ON c.ParentId = cte.Id
                        )
                        SELECT Id, Name, ParentId, Level
                        FROM CatCTE
                        ORDER BY Level, ParentId, Name;
                    END
                    """;

            await using var con = new SqlConnection(connectionString);
            await con.OpenAsync(ct);
            await using var cmd = con.CreateCommand();
            cmd.CommandText = sp;
            await cmd.ExecuteNonQueryAsync(ct);
        }

    }
}
