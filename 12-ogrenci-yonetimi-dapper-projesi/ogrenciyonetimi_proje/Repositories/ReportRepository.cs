using Dapper;
using ogrenciyonetimi_proje.Data;

namespace ogrenciyonetimi_proje.Repositories
{
    public interface IReportRepository
    {
        Task<IEnumerable<dynamic>> GetReportAsync(string reportType);
    }

    public class ReportRepository : IReportRepository
    {
        private readonly DbConnectionFactory _db;

        public ReportRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        public async Task<IEnumerable<dynamic>> GetReportAsync(string reportType)
        {
            using var conn = _db.CreateConnection();
            string query = reportType.ToLower() switch
            {
                "topgpa" => 
                    "SELECT TOP 5 s.StudentId, (s.FirstName + ' ' + s.LastName) AS StudentName, d.DepartmentName, s.GradeLevel, ROUND(AVG(g.Score), 2) as Value " +
                    "FROM Students s " +
                    "INNER JOIN Departments d ON s.DepartmentId = d.DepartmentId " +
                    "INNER JOIN Grades g ON s.StudentId = g.StudentId " +
                    "GROUP BY s.StudentId, s.FirstName, s.LastName, d.DepartmentName, s.GradeLevel " +
                    "ORDER BY Value DESC",

                "graduating" => 
                    "SELECT s.StudentId, (s.FirstName + ' ' + s.LastName) AS StudentName, d.DepartmentName, s.GradeLevel, ISNULL(ROUND(AVG(g.Score), 2), 0) as Value " +
                    "FROM Students s " +
                    "INNER JOIN Departments d ON s.DepartmentId = d.DepartmentId " +
                    "LEFT JOIN Grades g ON s.StudentId = g.StudentId " +
                    "WHERE s.GradeLevel = 4 " +
                    "GROUP BY s.StudentId, s.FirstName, s.LastName, d.DepartmentName, s.GradeLevel " +
                    "ORDER BY Value DESC",

                "failing" => 
                    "SELECT s.StudentId, (s.FirstName + ' ' + s.LastName) AS StudentName, d.DepartmentName, g.CourseName + ' (' + CAST(CAST(g.Score AS INT) AS VARCHAR) + ')' AS CourseDetail, g.Score AS Value " +
                    "FROM Students s " +
                    "INNER JOIN Departments d ON s.DepartmentId = d.DepartmentId " +
                    "INNER JOIN Grades g ON s.StudentId = g.StudentId " +
                    "WHERE g.Score < 50 " +
                    "ORDER BY g.Score ASC",

                "deptgpa" => 
                    "SELECT d.DepartmentName, COUNT(s.StudentId) as StudentCount, ISNULL(ROUND(AVG(g.Score), 2), 0) as Value " +
                    "FROM Departments d " +
                    "LEFT JOIN Students s ON d.DepartmentId = s.DepartmentId " +
                    "LEFT JOIN Grades g ON s.StudentId = g.StudentId " +
                    "GROUP BY d.DepartmentId, d.DepartmentName " +
                    "ORDER BY StudentCount DESC",

                "gradesbyclass" => 
                    "SELECT (CAST(GradeLevel AS VARCHAR) + '. Sınıf') AS GradeLevelName, COUNT(*) as StudentCount, SUM(CASE WHEN IsActive = 1 THEN 1 ELSE 0 END) as Value " +
                    "FROM Students " +
                    "GROUP BY GradeLevel " +
                    "ORDER BY GradeLevel",

                "mostgradedcourses" => 
                    "SELECT CourseName, COUNT(*) as GradeCount, ROUND(AVG(Score), 2) as Value " +
                    "FROM Grades " +
                    "GROUP BY CourseName " +
                    "ORDER BY GradeCount DESC",

                "inactivestudents" => 
                    "SELECT s.StudentId, (s.FirstName + ' ' + s.LastName) AS StudentName, d.DepartmentName, s.GradeLevel, s.Email AS Value " +
                    "FROM Students s " +
                    "INNER JOIN Departments d ON s.DepartmentId = d.DepartmentId " +
                    "WHERE s.IsActive = 0 " +
                    "ORDER BY StudentName",

                "topcourses" => 
                    "SELECT CourseName, ROUND(AVG(Score), 2) as Value, COUNT(*) as StudentCount " +
                    "FROM Grades " +
                    "GROUP BY CourseName " +
                    "ORDER BY Value DESC",

                "nogrades" => 
                    "SELECT s.StudentId, (s.FirstName + ' ' + s.LastName) AS StudentName, d.DepartmentName, s.GradeLevel, s.Email AS Value " +
                    "FROM Students s " +
                    "INNER JOIN Departments d ON s.DepartmentId = d.DepartmentId " +
                    "LEFT JOIN Grades g ON s.StudentId = g.StudentId " +
                    "WHERE g.GradeId IS NULL " +
                    "ORDER BY StudentName",

                "recentregistrations" => 
                    "SELECT s.StudentId, (s.FirstName + ' ' + s.LastName) AS StudentName, d.DepartmentName, s.GradeLevel, CONVERT(VARCHAR, s.CreatedAt, 104) AS Value " +
                    "FROM Students s " +
                    "INNER JOIN Departments d ON s.DepartmentId = d.DepartmentId " +
                    "WHERE s.CreatedAt >= DATEADD(day, -30, GETDATE()) " +
                    "ORDER BY s.CreatedAt DESC",

                _ => throw new ArgumentException("Geçersiz rapor türü.")
            };

            return await conn.QueryAsync<dynamic>(query);
        }
    }
}
