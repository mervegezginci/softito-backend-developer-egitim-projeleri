using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ogrenciyonetimi_proje.Models;
using ogrenciyonetimi_proje.Repositories;

namespace ogrenciyonetimi_proje.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentRepository _repo;
        private readonly ILogger<DepartmentsController> _logger;

        public DepartmentsController(IDepartmentRepository repo, ILogger<DepartmentsController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        /// <summary>Tüm bölümleri listele</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("API: Tüm bölümler listeleniyor.");
            var departments = await _repo.GetAllAsync();
            return Ok(departments);
        }

        /// <summary>ID ile bölüm getir</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("API: Bölüm detayları getiriliyor. ID: {Id}", id);
            var dept = await _repo.GetByIdAsync(id);
            if (dept == null) return NotFound(new { message = "Bölüm bulunamadı." });
            return Ok(dept);
        }

        /// <summary>Yeni bölüm ekle</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Department dept)
        {
            _logger.LogInformation("API: Yeni bölüm ekleme isteği alındı. İsim: {Name}", dept.DepartmentName);
            if (string.IsNullOrWhiteSpace(dept.DepartmentName))
                return BadRequest(new { message = "Bölüm adı boş olamaz." });

            var newId = await _repo.InsertAsync(dept.DepartmentName);
            return CreatedAtAction(nameof(GetById), new { id = newId }, new { id = newId, message = "Bölüm eklendi." });
        }

        /// <summary>Bölüm güncelle</summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Department dept)
        {
            _logger.LogInformation("API: Bölüm güncelleme isteği alındı. ID: {Id}", id);
            if (string.IsNullOrWhiteSpace(dept.DepartmentName))
                return BadRequest(new { message = "Bölüm adı boş olamaz." });

            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound(new { message = "Bölüm bulunamadı." });

            dept.DepartmentId = id;
            await _repo.UpdateAsync(dept);
            return Ok(new { message = "Bölüm güncellendi." });
        }

        /// <summary>Bölüm sil</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("API: Bölüm silme isteği alındı. ID: {Id}", id);
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound(new { message = "Bölüm bulunamadı." });

            try
            {
                await _repo.DeleteAsync(id);
                return Ok(new { message = "Bölüm silindi." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API Hata: Bölüm silinemedi. ID: {Id}", id);
                return BadRequest(new { message = "Bu bölüm silinemez. Bölüme kayıtlı öğrenciler olabilir.", details = ex.Message });
            }
        }
    }
}
