using ogrenciyonetimi_proje.Models;

namespace ogrenciyonetimi_proje.Repositories
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(int id);
        Task<IEnumerable<Student>> SearchAsync(string? keyword, int? departmentId);
        Task<int> InsertAsync(StudentCreateDto dto);
        Task UpdateAsync(int id, StudentUpdateDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<Grade>> GetGradesAsync(int studentId);
    }
}
