using Dapper;
using ogrenciyonetimi_proje.Data;
using ogrenciyonetimi_proje.Models;

namespace ogrenciyonetimi_proje.Repositories
{
    public interface IGradeRepository
    {
        Task<IEnumerable<Grade>> GetAllAsync();
        Task<Grade?> GetByIdAsync(int id);
        Task<IEnumerable<Grade>> GetByStudentIdAsync(int studentId);
        Task<int> InsertAsync(Grade grade);
        Task UpdateAsync(Grade grade);
        Task DeleteAsync(int id);
    }

    public class GradeRepository : IGradeRepository
    {
        private readonly DbConnectionFactory _db;

        public GradeRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Grade>> GetAllAsync()
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<Grade>(
                "SELECT g.GradeId, g.StudentId, g.CourseName, g.Score, g.CreatedAt, (s.FirstName + ' ' + s.LastName) AS StudentName " +
                "FROM Grades g INNER JOIN Students s ON g.StudentId = s.StudentId " +
                "ORDER BY g.CreatedAt DESC");
        }

        public async Task<Grade?> GetByIdAsync(int id)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Grade>(
                "SELECT g.GradeId, g.StudentId, g.CourseName, g.Score, g.CreatedAt, (s.FirstName + ' ' + s.LastName) AS StudentName " +
                "FROM Grades g INNER JOIN Students s ON g.StudentId = s.StudentId " +
                "WHERE g.GradeId = @Id",
                new { Id = id });
        }

        public async Task<IEnumerable<Grade>> GetByStudentIdAsync(int studentId)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<Grade>(
                "SELECT g.GradeId, g.StudentId, g.CourseName, g.Score, g.CreatedAt, (s.FirstName + ' ' + s.LastName) AS StudentName " +
                "FROM Grades g INNER JOIN Students s ON g.StudentId = s.StudentId " +
                "WHERE g.StudentId = @StudentId " +
                "ORDER BY g.CreatedAt DESC",
                new { StudentId = studentId });
        }

        public async Task<int> InsertAsync(Grade grade)
        {
            using var conn = _db.CreateConnection();
            return await conn.QuerySingleAsync<int>(
                "INSERT INTO Grades (StudentId, CourseName, Score, CreatedAt) VALUES (@StudentId, @CourseName, @Score, GETDATE()); SELECT CAST(SCOPE_IDENTITY() as int);",
                new { grade.StudentId, grade.CourseName, grade.Score });
        }

        public async Task UpdateAsync(Grade grade)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync(
                "UPDATE Grades SET CourseName = @CourseName, Score = @Score WHERE GradeId = @GradeId",
                new { grade.GradeId, grade.CourseName, grade.Score });
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync(
                "DELETE FROM Grades WHERE GradeId = @Id",
                new { Id = id });
        }
    }
}
