using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ogrenciyonetimi_proje.Models;
using ogrenciyonetimi_proje.Repositories;

namespace ogrenciyonetimi_proje.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GradesController : ControllerBase
    {
        private readonly IGradeRepository _repo;
        private readonly ILogger<GradesController> _logger;

        public GradesController(IGradeRepository repo, ILogger<GradesController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        /// <summary>Tüm notları listele</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("API: Tüm notlar listeleniyor.");
            var grades = await _repo.GetAllAsync();
            return Ok(grades);
        }

        /// <summary>ID ile not getir</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("API: Not detayı getiriliyor. ID: {Id}", id);
            var grade = await _repo.GetByIdAsync(id);
            if (grade == null) return NotFound(new { message = "Not kaydı bulunamadı." });
            return Ok(grade);
        }

        /// <summary>Öğrenciye göre notları listele</summary>
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetByStudentId(int studentId)
        {
            _logger.LogInformation("API: Öğrencinin notları listeleniyor. Öğrenci ID: {StudentId}", studentId);
            var grades = await _repo.GetByStudentIdAsync(studentId);
            return Ok(grades);
        }

        /// <summary>Yeni not ekle</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Grade grade)
        {
            _logger.LogInformation("API: Yeni not girişi isteği alındı. Öğrenci: {StudentId}, Ders: {Course}, Not: {Score}", grade.StudentId, grade.CourseName, grade.Score);
            if (grade.StudentId <= 0) return BadRequest(new { message = "Geçersiz öğrenci seçimi." });
            if (string.IsNullOrWhiteSpace(grade.CourseName)) return BadRequest(new { message = "Ders adı boş olamaz." });
            if (grade.Score < 0 || grade.Score > 100) return BadRequest(new { message = "Not 0 ile 100 arasında olmalıdır." });

            var newId = await _repo.InsertAsync(grade);
            return CreatedAtAction(nameof(GetById), new { id = newId }, new { id = newId, message = "Not eklendi." });
        }

        /// <summary>Not güncelle</summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Grade grade)
        {
            _logger.LogInformation("API: Not güncelleme isteği alındı. ID: {Id}", id);
            if (string.IsNullOrWhiteSpace(grade.CourseName)) return BadRequest(new { message = "Ders adı boş olamaz." });
            if (grade.Score < 0 || grade.Score > 100) return BadRequest(new { message = "Not 0 ile 100 arasında olmalıdır." });

            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound(new { message = "Not kaydı bulunamadı." });

            grade.GradeId = id;
            await _repo.UpdateAsync(grade);
            return Ok(new { message = "Not güncellendi." });
        }

        /// <summary>Not sil</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("API: Not silme isteği alındı. ID: {Id}", id);
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound(new { message = "Not kaydı bulunamadı." });

            await _repo.DeleteAsync(id);
            return Ok(new { message = "Not kaydı silindi." });
        }
    }
}
