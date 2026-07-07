using Dapper;
using ogrenciyonetimi_proje.Data;
using ogrenciyonetimi_proje.Models;

namespace ogrenciyonetimi_proje.Repositories
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllAsync();
    }

    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly DbConnectionFactory _db;

        public DepartmentRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<Department>(
                "sp_GetAllDepartments",
                commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
