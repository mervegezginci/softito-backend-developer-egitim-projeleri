using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using sporkulubu_mvc.Models;
using sporkulubu_mvc.Services;

namespace sporkulubu_mvc.Controllers
{
    [Authorize]
    public class TrainingController : Controller
    {
        private readonly ApiService _api;
        private readonly ILogger<TrainingController> _logger;

        public TrainingController(ApiService api, ILogger<TrainingController> logger)
        {
            _api = api;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Antrenman programları listesi görüntülendi.");
            var trainings = await _api.GetTrainingsAsync();
            return View(trainings);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Members = await _api.GetMembersAsync(isActive: true);
            ViewBag.Coaches = await _api.GetCoachesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TrainingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Members = await _api.GetMembersAsync(isActive: true);
                ViewBag.Coaches = await _api.GetCoachesAsync();
                return View(model);
            }

            var success = await _api.CreateTrainingAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Antrenman kaydı oluşturulurken API'de hata oluştu.");
                ViewBag.Members = await _api.GetMembersAsync(isActive: true);
                ViewBag.Coaches = await _api.GetCoachesAsync();
                return View(model);
            }

            _logger.LogInformation("Yeni antrenman seansı başarıyla planlandı.");
            TempData["Success"] = "Antrenman seansı başarıyla planlandı.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var training = await _api.GetTrainingByIdAsync(id);
            if (training == null) return NotFound();

            ViewBag.Members = await _api.GetMembersAsync(isActive: true);
            ViewBag.Coaches = await _api.GetCoachesAsync();
            return View(training);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, TrainingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Members = await _api.GetMembersAsync(isActive: true);
                ViewBag.Coaches = await _api.GetCoachesAsync();
                return View(model);
            }

            var success = await _api.UpdateTrainingAsync(id, model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Antrenman kaydı güncellenirken API'de hata oluştu.");
                ViewBag.Members = await _api.GetMembersAsync(isActive: true);
                ViewBag.Coaches = await _api.GetCoachesAsync();
                return View(model);
            }

            _logger.LogInformation("Antrenman kaydı başarıyla güncellendi. ID: {Id}", id);
            TempData["Success"] = "Antrenman kaydı başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _api.DeleteTrainingAsync(id);
            if (!success)
            {
                TempData["Error"] = "Antrenman kaydı silinemedi.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogInformation("Antrenman seans kaydı silindi. ID: {Id}", id);
            TempData["Success"] = "Antrenman seansı kaydı başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
