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
    public class BranchesController : ControllerBase
    {
        private readonly IBranchRepository _repo;
        private readonly ILogger<BranchesController> _logger;

        public BranchesController(IBranchRepository repo, ILogger<BranchesController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("API: Tüm spor branşları listeleniyor.");
            var branches = await _repo.GetAllAsync();
            return Ok(branches);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("API: Branş detayı isteniyor. ID: {Id}", id);
            var branch = await _repo.GetByIdAsync(id);
            if (branch == null) return NotFound(new { message = "Spor branşı bulunamadı." });
            return Ok(branch);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SportsBranch branch)
        {
            _logger.LogInformation("API: Yeni spor branşı ekleniyor. Ad: {Name}", branch.BranchName);
            if (string.IsNullOrWhiteSpace(branch.BranchName))
                return BadRequest(new { message = "Branş adı boş olamaz." });

            var newId = await _repo.InsertAsync(branch);
            return CreatedAtAction(nameof(GetById), new { id = newId }, new { id = newId, message = "Branş başarıyla oluşturuldu." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SportsBranch branch)
        {
            _logger.LogInformation("API: Branş güncelleniyor. ID: {Id}", id);
            if (string.IsNullOrWhiteSpace(branch.BranchName))
                return BadRequest(new { message = "Branş adı boş olamaz." });

            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound(new { message = "Spor branşı bulunamadı." });

            branch.BranchId = id;
            await _repo.UpdateAsync(branch);
            return Ok(new { message = "Branş başarıyla güncellendi." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("API: Branş siliniyor. ID: {Id}", id);
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound(new { message = "Spor branşı bulunamadı." });

            try
            {
                await _repo.DeleteAsync(id);
                return Ok(new { message = "Branş başarıyla silindi." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API Hata: Branş silinemedi. ID: {Id}", id);
                return BadRequest(new { message = "Bu branş silinemez. Bu branşa atanmış antrenörler veya antrenmanlar mevcut.", details = ex.Message });
            }
        }
    }
}
