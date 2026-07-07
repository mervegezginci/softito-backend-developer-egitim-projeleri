using Dapper;
using ogrenciyonetimi_proje.Data;
using ogrenciyonetimi_proje.Models;

namespace ogrenciyonetimi_proje.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly DbConnectionFactory _db;

        public StudentRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<Student>(
                "sp_GetAllStudents",
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Student>(
                "sp_GetStudentById",
                new { StudentId = id },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Student>> SearchAsync(string? keyword, int? departmentId)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<Student>(
                "sp_SearchStudents",
                new { Keyword = keyword, DepartmentId = departmentId },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<int> InsertAsync(StudentCreateDto dto)
        {
            using var conn = _db.CreateConnection();
            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "sp_InsertStudent",
                new
                {
                    dto.FirstName,
                    dto.LastName,
                    dto.Email,
                    dto.Phone,
                    dto.DepartmentId,
                    dto.GradeLevel
                },
                commandType: System.Data.CommandType.StoredProcedure);
            return (int)(result?.NewId ?? 0);
        }

        public async Task UpdateAsync(int id, StudentUpdateDto dto)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync(
                "sp_UpdateStudent",
                new
                {
                    StudentId = id,
                    dto.FirstName,
                    dto.LastName,
                    dto.Email,
                    dto.Phone,
                    dto.DepartmentId,
                    dto.GradeLevel,
                    dto.IsActive
                },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync(
                "sp_DeleteStudent",
                new { StudentId = id },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Grade>> GetGradesAsync(int studentId)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<Grade>(
                "sp_GetGradesByStudent",
                new { StudentId = studentId },
                commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
