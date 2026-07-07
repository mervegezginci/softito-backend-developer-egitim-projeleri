using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using sporkulubu_proje.Data;
using sporkulubu_proje.Models;

namespace sporkulubu_proje.Repositories
{
    // --- Interfaces ---

    public interface IBranchRepository
    {
        Task<IEnumerable<SportsBranch>> GetAllAsync();
        Task<SportsBranch?> GetByIdAsync(int id);
        Task<int> InsertAsync(SportsBranch branch);
        Task UpdateAsync(SportsBranch branch);
        Task DeleteAsync(int id);
    }

    public interface ICoachRepository
    {
        Task<IEnumerable<Coach>> GetAllAsync();
        Task<Coach?> GetByIdAsync(int id);
        Task<int> InsertAsync(CoachDto coach);
        Task UpdateAsync(int id, CoachDto coach);
        Task DeleteAsync(int id);
    }

    public interface IMemberRepository
    {
        Task<IEnumerable<Member>> GetAllAsync();
        Task<Member?> GetByIdAsync(int id);
        Task<IEnumerable<Member>> SearchAsync(string? keyword, bool? isActive);
        Task<int> InsertAsync(MemberDto member);
        Task UpdateAsync(int id, MemberDto member);
        Task DeleteAsync(int id);
    }

    public interface ITrainingRepository
    {
        Task<IEnumerable<Training>> GetAllAsync();
        Task<Training?> GetByIdAsync(int id);
        Task<int> InsertAsync(TrainingDto training);
        Task UpdateAsync(int id, TrainingDto training);
        Task DeleteAsync(int id);
    }

    public interface IReportRepository
    {
        Task<IEnumerable<dynamic>> GetReportAsync(string reportType);
    }

    // --- Implementations ---

    public class BranchRepository : IBranchRepository
    {
        private readonly DbConnectionFactory _db;
        public BranchRepository(DbConnectionFactory db) => _db = db;

        public async Task<IEnumerable<SportsBranch>> GetAllAsync()
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<SportsBranch>("sp_GetBranches", commandType: CommandType.StoredProcedure);
        }

        public async Task<SportsBranch?> GetByIdAsync(int id)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<SportsBranch>("sp_GetBranchById", new { BranchId = id }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertAsync(SportsBranch branch)
        {
            using var conn = _db.CreateConnection();
            return await conn.QuerySingleAsync<int>("sp_InsertBranch", new { branch.BranchName, branch.Description }, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(SportsBranch branch)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("sp_UpdateBranch", new { branch.BranchId, branch.BranchName, branch.Description }, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("sp_DeleteBranch", new { BranchId = id }, commandType: CommandType.StoredProcedure);
        }
    }

    public class CoachRepository : ICoachRepository
    {
        private readonly DbConnectionFactory _db;
        public CoachRepository(DbConnectionFactory db) => _db = db;

        public async Task<IEnumerable<Coach>> GetAllAsync()
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<Coach>("sp_GetCoaches", commandType: CommandType.StoredProcedure);
        }

        public async Task<Coach?> GetByIdAsync(int id)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Coach>("sp_GetCoachById", new { CoachId = id }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertAsync(CoachDto coach)
        {
            using var conn = _db.CreateConnection();
            return await conn.QuerySingleAsync<int>("sp_InsertCoach", new { coach.FirstName, coach.LastName, coach.Email, coach.Phone, coach.BranchId }, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(int id, CoachDto coach)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("sp_UpdateCoach", new { CoachId = id, coach.FirstName, coach.LastName, coach.Email, coach.Phone, coach.BranchId }, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("sp_DeleteCoach", new { CoachId = id }, commandType: CommandType.StoredProcedure);
        }
    }

    public class MemberRepository : IMemberRepository
    {
        private readonly DbConnectionFactory _db;
        public MemberRepository(DbConnectionFactory db) => _db = db;

        public async Task<IEnumerable<Member>> GetAllAsync()
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<Member>("sp_GetMembers", commandType: CommandType.StoredProcedure);
        }

        public async Task<Member?> GetByIdAsync(int id)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Member>("sp_GetMemberById", new { MemberId = id }, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Member>> SearchAsync(string? keyword, bool? isActive)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<Member>("sp_SearchMembers", new { Keyword = keyword, IsActive = isActive }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertAsync(MemberDto member)
        {
            using var conn = _db.CreateConnection();
            return await conn.QuerySingleAsync<int>("sp_InsertMember", new { member.FirstName, member.LastName, member.Email, member.Phone }, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(int id, MemberDto member)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("sp_UpdateMember", new { MemberId = id, member.FirstName, member.LastName, member.Email, member.Phone, member.IsActive }, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("sp_DeleteMember", new { MemberId = id }, commandType: CommandType.StoredProcedure);
        }
    }

    public class TrainingRepository : ITrainingRepository
    {
        private readonly DbConnectionFactory _db;
        public TrainingRepository(DbConnectionFactory db) => _db = db;

        public async Task<IEnumerable<Training>> GetAllAsync()
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<Training>("sp_GetTrainings", commandType: CommandType.StoredProcedure);
        }

        public async Task<Training?> GetByIdAsync(int id)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Training>("sp_GetTrainingById", new { TrainingId = id }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertAsync(TrainingDto training)
        {
            using var conn = _db.CreateConnection();
            return await conn.QuerySingleAsync<int>("sp_InsertTraining", new { training.MemberId, training.CoachId, training.TrainingDate, training.DurationMinutes, training.Fee }, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(int id, TrainingDto training)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("sp_UpdateTraining", new { TrainingId = id, training.MemberId, training.CoachId, training.TrainingDate, training.DurationMinutes, training.Fee }, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("sp_DeleteTraining", new { TrainingId = id }, commandType: CommandType.StoredProcedure);
        }
    }

    public class ReportRepository : IReportRepository
    {
        private readonly DbConnectionFactory _db;
        public ReportRepository(DbConnectionFactory db) => _db = db;

        public async Task<IEnumerable<dynamic>> GetReportAsync(string reportType)
        {
            using var conn = _db.CreateConnection();
            string query = reportType.ToLower() switch
            {
                "topgpa" => 
                    "SELECT TOP 5 m.MemberId, (m.FirstName + ' ' + m.LastName) AS StudentName, b.BranchName AS DepartmentName, COUNT(t.TrainingId) AS GradeLevel, ROUND(SUM(t.Fee), 2) as Value " +
                    "FROM Members m " +
                    "LEFT JOIN Trainings t ON m.MemberId = t.MemberId " +
                    "LEFT JOIN Coaches c ON t.CoachId = c.CoachId " +
                    "LEFT JOIN SportsBranches b ON c.BranchId = b.BranchId " +
                    "GROUP BY m.MemberId, m.FirstName, m.LastName, b.BranchName " +
                    "ORDER BY Value DESC",

                "graduating" => 
                    "SELECT TOP 5 c.CoachId AS StudentId, (c.FirstName + ' ' + c.LastName) AS StudentName, b.BranchName AS DepartmentName, COUNT(t.TrainingId) AS GradeLevel, ROUND(SUM(t.Fee), 2) as Value " +
                    "FROM Coaches c " +
                    "LEFT JOIN Trainings t ON c.CoachId = t.CoachId " +
                    "LEFT JOIN SportsBranches b ON c.BranchId = b.BranchId " +
                    "GROUP BY c.CoachId, c.FirstName, c.LastName, b.BranchName " +
                    "ORDER BY GradeLevel DESC",

                "failing" => 
                    "SELECT m.MemberId AS StudentId, (m.FirstName + ' ' + m.LastName) AS StudentName, b.BranchName AS DepartmentName, (c.FirstName + ' ' + c.LastName + ' - ' + CONVERT(VARCHAR, t.TrainingDate, 104)) AS CourseDetail, t.Fee AS Value " +
                    "FROM Trainings t " +
                    "INNER JOIN Members m ON t.MemberId = m.MemberId " +
                    "INNER JOIN Coaches c ON t.CoachId = c.CoachId " +
                    "INNER JOIN SportsBranches b ON c.BranchId = b.BranchId " +
                    "WHERE t.Fee > 300 " +
                    "ORDER BY t.Fee DESC",

                "deptgpa" => 
                    "SELECT b.BranchName AS DepartmentName, COUNT(t.TrainingId) as StudentCount, ISNULL(ROUND(SUM(t.Fee), 2), 0) as Value " +
                    "FROM SportsBranches b " +
                    "LEFT JOIN Coaches c ON b.BranchId = c.BranchId " +
                    "LEFT JOIN Trainings t ON c.CoachId = t.CoachId " +
                    "GROUP BY b.BranchId, b.BranchName " +
                    "ORDER BY StudentCount DESC",

                "gradesbyclass" => 
                    "SELECT (CASE WHEN IsActive = 1 THEN N'Aktif Üyeler' ELSE N'Pasif Üyeler' END) AS GradeLevelName, COUNT(*) as StudentCount, SUM(CASE WHEN IsActive = 1 THEN 1 ELSE 0 END) as Value " +
                    "FROM Members " +
                    "GROUP BY IsActive",

                "mostgradedcourses" => 
                    "SELECT (c.FirstName + ' ' + c.LastName) AS CourseName, COUNT(t.TrainingId) as GradeCount, ISNULL(ROUND(AVG(t.Fee), 2), 0) as Value " +
                    "FROM Coaches c " +
                    "LEFT JOIN Trainings t ON c.CoachId = t.CoachId " +
                    "GROUP BY c.CoachId, c.FirstName, c.LastName " +
                    "ORDER BY GradeCount DESC",

                "inactivestudents" => 
                    "SELECT m.MemberId AS StudentId, (m.FirstName + ' ' + m.LastName) AS StudentName, N'Pasif Üye' AS DepartmentName, 0 AS GradeLevel, m.Email AS Value " +
                    "FROM Members m " +
                    "WHERE m.IsActive = 0 " +
                    "ORDER BY StudentName",

                "topcourses" => 
                    "SELECT b.BranchName AS CourseName, COUNT(t.TrainingId) as StudentCount, ISNULL(ROUND(AVG(t.Fee), 2), 0) as Value " +
                    "FROM SportsBranches b " +
                    "LEFT JOIN Coaches c ON b.BranchId = c.BranchId " +
                    "LEFT JOIN Trainings t ON c.CoachId = t.CoachId " +
                    "GROUP BY b.BranchId, b.BranchName " +
                    "ORDER BY StudentCount DESC",

                "nogrades" => 
                    "SELECT m.MemberId AS StudentId, (m.FirstName + ' ' + m.LastName) AS StudentName, N'Branşsız / Seanssız' AS DepartmentName, 1 AS GradeLevel, m.Email AS Value " +
                    "FROM Members m " +
                    "LEFT JOIN Trainings t ON m.MemberId = t.MemberId " +
                    "WHERE t.TrainingId IS NULL " +
                    "ORDER BY StudentName",

                "recentregistrations" => 
                    "SELECT m.MemberId AS StudentId, (m.FirstName + ' ' + m.LastName) AS StudentName, N'Yeni Kayıt' AS DepartmentName, 1 AS GradeLevel, CONVERT(VARCHAR, m.JoinDate, 104) AS Value " +
                    "FROM Members m " +
                    "WHERE m.JoinDate >= DATEADD(day, -30, GETDATE()) " +
                    "ORDER BY m.JoinDate DESC",

                _ => throw new ArgumentException("Geçersiz rapor türü.")
            };

            return await conn.QueryAsync<dynamic>(query);
        }
    }
}
