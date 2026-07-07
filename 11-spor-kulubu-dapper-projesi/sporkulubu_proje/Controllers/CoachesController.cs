using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using sporkulubu_proje.Models;
using sporkulubu_proje.Repositories;

namespace sporkulubu_proje.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CoachesController : ControllerBase
    {
        private readonly ICoachRepository _repo;
        private readonly ILogger<CoachesController> _logger;

        public CoachesController(ICoachRepository repo, ILogger<CoachesController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("API: Tüm antrenörler listeleniyor.");
            var coaches = await _repo.GetAllAsync();
            return Ok(coaches);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("API: Antrenör detayı isteniyor. ID: {Id}", id);
            var coach = await _repo.GetByIdAsync(id);
            if (coach == null) return NotFound(new { message = "Antrenör bulunamadı." });
            return Ok(coach);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CoachDto coach)
        {
            _logger.LogInformation("API: Yeni antrenör ekleniyor. E-posta: {Email}", coach.Email);
            if (string.IsNullOrWhiteSpace(coach.FirstName) || string.IsNullOrWhiteSpace(coach.LastName))
                return BadRequest(new { message = "Ad ve soyad boş olamaz." });
            if (coach.BranchId <= 0)
                return BadRequest(new { message = "Geçersiz branş seçimi." });

            try
            {
                var newId = await _repo.InsertAsync(coach);
                return CreatedAtAction(nameof(GetById), new { id = newId }, new { id = newId, message = "Antrenör başarıyla eklendi." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API Hata: Antrenör eklenemedi.");
                return BadRequest(new { message = "Antrenör eklenirken hata oluştu. E-posta adresi kullanımda olabilir.", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CoachDto coach)
        {
            _logger.LogInformation("API: Antrenör güncelleniyor. ID: {Id}", id);
            if (string.IsNullOrWhiteSpace(coach.FirstName) || string.IsNullOrWhiteSpace(coach.LastName))
                return BadRequest(new { message = "Ad ve soyad boş olamaz." });
            if (coach.BranchId <= 0)
                return BadRequest(new { message = "Geçersiz branş seçimi." });

            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound(new { message = "Antrenör bulunamadı." });

            await _repo.UpdateAsync(id, coach);
            return Ok(new { message = "Antrenör bilgileri başarıyla güncellendi." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("API: Antrenör siliniyor. ID: {Id}", id);
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound(new { message = "Antrenör bulunamadı." });

            try
            {
                await _repo.DeleteAsync(id);
                return Ok(new { message = "Antrenör başarıyla silindi." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API Hata: Antrenör silinemedi. ID: {Id}", id);
                return BadRequest(new { message = "Antrenör silinemedi. Atanmış antrenman programları olabilir.", details = ex.Message });
            }
        }
    }
}
