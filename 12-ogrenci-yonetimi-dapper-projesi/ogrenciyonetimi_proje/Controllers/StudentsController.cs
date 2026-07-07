using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ogrenciyonetimi_proje.Models;
using ogrenciyonetimi_proje.Repositories;
using ogrenciyonetimi_proje.Services;

namespace ogrenciyonetimi_proje.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository _repo;
        private readonly IPdfService _pdf;
        private readonly IExcelService _excel;

        public StudentsController(IStudentRepository repo, IPdfService pdf, IExcelService excel)
        {
            _repo = repo;
            _pdf = pdf;
            _excel = excel;
        }

        /// <summary>Tüm öğrencileri listele</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var students = await _repo.GetAllAsync();
            return Ok(students);
        }

        /// <summary>ID ile öğrenci getir</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var student = await _repo.GetByIdAsync(id);
            if (student == null) return NotFound(new { message = "Öğrenci bulunamadı." });
            return Ok(student);
        }

        /// <summary>Öğrenci arama (keyword, departmentId ile)</summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] int? deptId)
        {
            var students = await _repo.SearchAsync(q, deptId);
            return Ok(students);
        }

        /// <summary>Yeni öğrenci ekle</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StudentCreateDto dto)
        {
            var newId = await _repo.InsertAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = newId }, new { id = newId, message = "Öğrenci eklendi." });
        }

        /// <summary>Öğrenci güncelle</summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] StudentUpdateDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound(new { message = "Öğrenci bulunamadı." });
            await _repo.UpdateAsync(id, dto);
            return Ok(new { message = "Öğrenci güncellendi." });
        }

        /// <summary>Öğrenci sil</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound(new { message = "Öğrenci bulunamadı." });
            await _repo.DeleteAsync(id);
            return Ok(new { message = "Öğrenci silindi." });
        }

        /// <summary>Öğrencinin notlarını getir</summary>
        [HttpGet("{id}/grades")]
        public async Task<IActionResult> GetGrades(int id)
        {
            var grades = await _repo.GetGradesAsync(id);
            return Ok(grades);
        }

        /// <summary>Öğrenci listesini PDF olarak indir</summary>
        [HttpGet("export/pdf")]
        public async Task<IActionResult> ExportPdf()
        {
            var students = await _repo.GetAllAsync();
            var bytes = _pdf.GenerateStudentListPdf(students);
            return File(bytes, "application/pdf", $"ogrenciler_{DateTime.Now:yyyyMMdd}.pdf");
        }

        /// <summary>Öğrenci listesini Excel olarak indir</summary>
        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportExcel()
        {
            var students = await _repo.GetAllAsync();
            var bytes = _excel.GenerateStudentListExcel(students);
            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"ogrenciler_{DateTime.Now:yyyyMMdd}.xlsx");
        }
    }
}
