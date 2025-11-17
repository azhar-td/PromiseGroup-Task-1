using Domain.Abstractions;
using Domain.Entities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Infrastructure.Sql
{
    public sealed class SpCategoryTreeBuilder : ICategoryTreeBuilder
    {
        private readonly string _connectionString;
        public string Name => "StoredProc";

        public SpCategoryTreeBuilder(string connectionString) => _connectionString = connectionString;

        public async Task<IReadOnlyList<TreeNode>> BuildAsync(CancellationToken ct = default)
        {
            var rows = new List<(int Id, string Name, int? ParentId, int Level)>();

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("dbo.GetCategoryTree", con) { CommandType = CommandType.StoredProcedure };
            await con.OpenAsync(ct);
            using var rdr = await cmd.ExecuteReaderAsync(ct);
            while (await rdr.ReadAsync(ct))
            {
                rows.Add((
                    rdr.GetInt32(rdr.GetOrdinal("Id")),
                    rdr.GetString(rdr.GetOrdinal("Name")),
                    rdr.IsDBNull(rdr.GetOrdinal("ParentId")) ? null : rdr.GetInt32(rdr.GetOrdinal("ParentId")),
                    rdr.GetInt32(rdr.GetOrdinal("Level"))
                ));
            }

            // rebuild hierarchy from flat (Id/ParentId/Level)
            var byId = rows.ToDictionary(r => r.Id, r => new TreeNode(r.Id, r.Name, r.Level, new List<TreeNode>()));
            var roots = new List<TreeNode>();

            foreach (var r in rows.OrderBy(r => r.Level))
            {
                var node = (List<TreeNode>)byId[r.Id].Children;
                // (Children list is List<TreeNode> via construction above)
                if (r.ParentId is null)
                    roots.Add(byId[r.Id]);
                else
                    ((List<TreeNode>)byId[r.ParentId.Value].Children).Add(byId[r.Id]);
            }

            return roots;
        }

    }
}
