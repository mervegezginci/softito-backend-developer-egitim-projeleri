using Dapper;
using ogrenciyonetimi_proje.Data;
using ogrenciyonetimi_proje.Models;

namespace ogrenciyonetimi_proje.Repositories
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllAsync();
        Task<Department?> GetByIdAsync(int id);
        Task<int> InsertAsync(string name);
        Task UpdateAsync(Department department);
        Task DeleteAsync(int id);
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

        public async Task<Department?> GetByIdAsync(int id)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Department>(
                "SELECT DepartmentId, DepartmentName FROM Departments WHERE DepartmentId = @Id",
                new { Id = id });
        }

        public async Task<int> InsertAsync(string name)
        {
            using var conn = _db.CreateConnection();
            return await conn.QuerySingleAsync<int>(
                "INSERT INTO Departments (DepartmentName) VALUES (@Name); SELECT CAST(SCOPE_IDENTITY() as int);",
                new { Name = name });
        }

        public async Task UpdateAsync(Department department)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync(
                "UPDATE Departments SET DepartmentName = @DepartmentName WHERE DepartmentId = @DepartmentId",
                department);
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync(
                "DELETE FROM Departments WHERE DepartmentId = @Id",
                new { Id = id });
        }
    }
}
