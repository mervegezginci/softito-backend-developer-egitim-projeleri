using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ogrenciyonetimi_mvc.Models;
using ogrenciyonetimi_mvc.Services;

namespace ogrenciyonetimi_mvc.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly ApiService _api;

        public StudentController(ApiService api)
        {
            _api = api;
        }

        // GET: /Student
        public async Task<IActionResult> Index(string? q, int? deptId)
        {
            var students = await _api.GetStudentsAsync(q, deptId);
            ViewBag.Departments = await _api.GetDepartmentsAsync();
            ViewBag.SearchQ = q;
            ViewBag.DeptId = deptId;
            return View(students);
        }

        // GET: /Student/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Departments = await _api.GetDepartmentsAsync();
            return View(new StudentViewModel { GradeLevel = 1 });
        }

        // POST: /Student/Create
        [HttpPost]
        public async Task<IActionResult> Create(StudentViewModel model)
        {
            var success = await _api.CreateStudentAsync(model);
            if (!success)
            {
                ModelState.AddModelError("", "Öğrenci eklenirken bir hata oluştu.");
                ViewBag.Departments = await _api.GetDepartmentsAsync();
                return View(model);
            }

            TempData["Success"] = "Öğrenci başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Student/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _api.GetStudentByIdAsync(id);
            if (student == null) return NotFound();

            ViewBag.Departments = await _api.GetDepartmentsAsync();
            return View(student);
        }

        // POST: /Student/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, StudentViewModel model)
        {
            var success = await _api.UpdateStudentAsync(id, model);
            if (!success)
            {
                ModelState.AddModelError("", "Öğrenci güncellenirken bir hata oluştu.");
                ViewBag.Departments = await _api.GetDepartmentsAsync();
                return View(model);
            }

            TempData["Success"] = "Öğrenci bilgileri güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Student/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _api.DeleteStudentAsync(id);
            if (success)
            {
                TempData["Success"] = "Öğrenci silindi.";
            }
            else
            {
                TempData["Error"] = "Öğrenci silinirken hata oluştu.";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: /Student/ExportPdf
        public async Task<IActionResult> ExportPdf()
        {
            var bytes = await _api.DownloadPdfAsync();
            if (bytes == null) return BadRequest("PDF oluşturulamadı.");
            return File(bytes, "application/pdf", $"ogrenciler_{DateTime.Now:yyyyMMdd}.pdf");
        }

        // GET: /Student/ExportExcel
        public async Task<IActionResult> ExportExcel()
        {
            var bytes = await _api.DownloadExcelAsync();
            if (bytes == null) return BadRequest("Excel oluşturulamadı.");
            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"ogrenciler_{DateTime.Now:yyyyMMdd}.xlsx");
        }
    }
}
