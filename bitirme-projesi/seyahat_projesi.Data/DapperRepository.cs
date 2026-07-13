using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using seyahat_projesi.Model;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace seyahat_projesi.Data
{
    public class DapperRepository
    {
        private readonly string _connectionString;

        public DapperRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? "Server=(localdb)\\mssqllocaldb;Database=seyahatdb;Trusted_Connection=True;MultipleActiveResultSets=true";
        }

        public string GetConnectionString() => _connectionString;

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null)
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<T>(sql, WrapParameters(parameters));
        }

        public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null)
        {
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(sql, WrapParameters(parameters));
        }

        public async Task<int> ExecuteAsync(string sql, object? parameters = null)
        {
            using var connection = CreateConnection();
            return await connection.ExecuteAsync(sql, WrapParameters(parameters));
        }

        // Multi-mapping helper for Tour + Category + Guide
        public async Task<IEnumerable<Tour>> QueryToursAsync(string sql, object? parameters = null)
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<Tour, Category, Guide, Tour>(sql, (tour, cat, guide) =>
            {
                tour.Category = cat;
                tour.Guide = guide;
                return tour;
            }, WrapParameters(parameters), splitOn: "Id,Id");
        }

        private object? WrapParameters(object? parameters)
        {
            if (parameters is Dictionary<string, object> dict)
            {
                var dbParams = new DynamicParameters();
                foreach (var kvp in dict)
                {
                    dbParams.Add(kvp.Key, kvp.Value);
                }
                return dbParams;
            }
            return parameters;
        }
    }
}
