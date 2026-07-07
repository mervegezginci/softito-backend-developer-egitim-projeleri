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
    public class TrainingsController : ControllerBase
    {
        private readonly ITrainingRepository _repo;
        private readonly ILogger<TrainingsController> _logger;

        public TrainingsController(ITrainingRepository repo, ILogger<TrainingsController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("API: Tüm antrenman kayıtları listeleniyor.");
            var trainings = await _repo.GetAllAsync();
            return Ok(trainings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("API: Antrenman kaydı detayı isteniyor. ID: {Id}", id);
            var training = await _repo.GetByIdAsync(id);
            if (training == null) return NotFound(new { message = "Antrenman kaydı bulunamadı." });
            return Ok(training);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TrainingDto training)
        {
            _logger.LogInformation("API: Yeni antrenman kaydı oluşturuluyor. Üye ID: {MemberId}, Tarih: {Date}", training.MemberId, training.TrainingDate);
            if (training.MemberId <= 0 || training.CoachId <= 0)
                return BadRequest(new { message = "Üye ve Antrenör seçimi zorunludur." });
            if (training.DurationMinutes <= 0)
                return BadRequest(new { message = "Seans süresi sıfırdan büyük olmalıdır." });

            var newId = await _repo.InsertAsync(training);
            return CreatedAtAction(nameof(GetById), new { id = newId }, new { id = newId, message = "Antrenman kaydı başarıyla oluşturuldu." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TrainingDto training)
        {
            _logger.LogInformation("API: Antrenman kaydı güncelleniyor. ID: {Id}", id);
            if (training.MemberId <= 0 || training.CoachId <= 0)
                return BadRequest(new { message = "Üye ve Antrenör seçimi zorunludur." });
            if (training.DurationMinutes <= 0)
                return BadRequest(new { message = "Seans süresi sıfırdan büyük olmalıdır." });

            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound(new { message = "Antrenman kaydı bulunamadı." });

            await _repo.UpdateAsync(id, training);
            return Ok(new { message = "Antrenman kaydı başarıyla güncellendi." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("API: Antrenman kaydı siliniyor. ID: {Id}", id);
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound(new { message = "Antrenman kaydı bulunamadı." });

            await _repo.DeleteAsync(id);
            return Ok(new { message = "Antrenman kaydı başarıyla silindi." });
        }
    }
}
