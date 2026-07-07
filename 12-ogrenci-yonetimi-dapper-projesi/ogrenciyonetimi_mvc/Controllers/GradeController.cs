using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ogrenciyonetimi_mvc.Models;
using ogrenciyonetimi_mvc.Services;

namespace ogrenciyonetimi_mvc.Controllers
{
    [Authorize]
    public class GradeController : Controller
    {
        private readonly ApiService _api;
        private readonly ILogger<GradeController> _logger;

        public GradeController(ApiService api, ILogger<GradeController> _logger)
        {
            _api = api;
            this._logger = _logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Not listesi görüntülendi.");
            var grades = await _api.GetGradesAsync();
            return View(grades);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var students = await _api.GetStudentsAsync();
            ViewBag.Students = students;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(GradeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var students = await _api.GetStudentsAsync();
                ViewBag.Students = students;
                return View(model);
            }

            var success = await _api.CreateGradeAsync(model);
            if (!success)
            {
                ModelState.AddModelError("", "Not kaydı eklenirken API'de hata oluştu.");
                var students = await _api.GetStudentsAsync();
                ViewBag.Students = students;
                return View(model);
            }

            _logger.LogInformation("Yeni not başarıyla eklendi: Öğrenci ID: {StudentId}, Ders: {Course}", model.StudentId, model.CourseName);
            TempData["Success"] = "Not başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var grade = await _api.GetGradeByIdAsync(id);
            if (grade == null) return NotFound();

            var students = await _api.GetStudentsAsync();
            ViewBag.Students = students;
            return View(grade);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, GradeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var students = await _api.GetStudentsAsync();
                ViewBag.Students = students;
                return View(model);
            }

            var success = await _api.UpdateGradeAsync(id, model);
            if (!success)
            {
                ModelState.AddModelError("", "Not kaydı güncellenirken API'de hata oluştu.");
                var students = await _api.GetStudentsAsync();
                ViewBag.Students = students;
                return View(model);
            }

            _logger.LogInformation("Not kaydı başarıyla güncellendi: Not ID: {GradeId}", id);
            TempData["Success"] = "Not başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _api.DeleteGradeAsync(id);
            if (!success)
            {
                TempData["Error"] = "Not kaydı silinemedi.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogInformation("Not kaydı başarıyla silindi: Not ID: {GradeId}", id);
            TempData["Success"] = "Not başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
