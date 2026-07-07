using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using sporkulubu_proje.Models;
using sporkulubu_proje.Repositories;
using sporkulubu_proje.Services;

namespace sporkulubu_proje.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MembersController : ControllerBase
    {
        private readonly IMemberRepository _repo;
        private readonly IPdfService _pdf;
        private readonly IExcelService _excel;
        private readonly ILogger<MembersController> _logger;

        public MembersController(IMemberRepository repo, IPdfService pdf, IExcelService excel, ILogger<MembersController> logger)
        {
            _repo = repo;
            _pdf = pdf;
            _excel = excel;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("API: Tüm kulüp üyeleri listeleniyor.");
            var members = await _repo.GetAllAsync();
            return Ok(members);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("API: Üye detayı isteniyor. ID: {Id}", id);
            var member = await _repo.GetByIdAsync(id);
            if (member == null) return NotFound(new { message = "Üye bulunamadı." });
            return Ok(member);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] bool? isActive)
        {
            _logger.LogInformation("API: Üye arama yapılıyor. Anahtar Kelime: {Query}, Durum: {IsActive}", q, isActive);
            var members = await _repo.SearchAsync(q, isActive);
            return Ok(members);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MemberDto member)
        {
            _logger.LogInformation("API: Yeni üye kaydı yapılıyor. E-posta: {Email}", member.Email);
            if (string.IsNullOrWhiteSpace(member.FirstName) || string.IsNullOrWhiteSpace(member.LastName))
                return BadRequest(new { message = "Ad ve soyad boş olamaz." });

            try
            {
                var newId = await _repo.InsertAsync(member);
                return CreatedAtAction(nameof(GetById), new { id = newId }, new { id = newId, message = "Üye başarıyla kaydedildi." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API Hata: Üye eklenemedi.");
                return BadRequest(new { message = "Üye eklenirken hata oluştu. E-posta adresi kullanımda olabilir.", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MemberDto member)
        {
            _logger.LogInformation("API: Üye bilgileri güncelleniyor. ID: {Id}", id);
            if (string.IsNullOrWhiteSpace(member.FirstName) || string.IsNullOrWhiteSpace(member.LastName))
                return BadRequest(new { message = "Ad ve soyad boş olamaz." });

            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound(new { message = "Üye bulunamadı." });

            await _repo.UpdateAsync(id, member);
            return Ok(new { message = "Üye bilgileri başarıyla güncellendi." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("API: Üye kaydı siliniyor. ID: {Id}", id);
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound(new { message = "Üye bulunamadı." });

            await _repo.DeleteAsync(id);
            return Ok(new { message = "Üye kaydı başarıyla silindi." });
        }

        [HttpGet("export/pdf")]
        public async Task<IActionResult> ExportPdf()
        {
            _logger.LogInformation("API: Üye listesi PDF raporu hazırlanıyor.");
            var members = await _repo.GetAllAsync();
            var pdfBytes = _pdf.GenerateMemberListPdf(members);
            return File(pdfBytes, "application/pdf", $"Uye_Listesi_{DateTime.Now:yyyyMMdd}.pdf");
        }

        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportExcel()
        {
            _logger.LogInformation("API: Üye listesi Excel raporu hazırlanıyor.");
            var members = await _repo.GetAllAsync();
            var excelBytes = _excel.GenerateMemberListExcel(members);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Uye_Listesi_{DateTime.Now:yyyyMMdd}.xlsx");
        }
    }
}
