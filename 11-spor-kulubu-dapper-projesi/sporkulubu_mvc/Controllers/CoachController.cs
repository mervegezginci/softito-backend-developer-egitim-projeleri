using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using sporkulubu_mvc.Models;
using sporkulubu_mvc.Services;

namespace sporkulubu_mvc.Controllers
{
    [Authorize]
    public class CoachController : Controller
    {
        private readonly ApiService _api;
        private readonly ILogger<CoachController> _logger;

        public CoachController(ApiService api, ILogger<CoachController> logger)
        {
            _api = api;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Antrenör listesi görüntülendi.");
            var coaches = await _api.GetCoachesAsync();
            return View(coaches);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Branches = await _api.GetBranchesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CoachViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Branches = await _api.GetBranchesAsync();
                return View(model);
            }

            var success = await _api.CreateCoachAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Antrenör eklenirken API'de hata oluştu. E-posta adresi kullanımda olabilir.");
                ViewBag.Branches = await _api.GetBranchesAsync();
                return View(model);
            }

            _logger.LogInformation("Yeni antrenör başarıyla eklendi.");
            TempData["Success"] = "Antrenör başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var coach = await _api.GetCoachByIdAsync(id);
            if (coach == null) return NotFound();

            ViewBag.Branches = await _api.GetBranchesAsync();
            return View(coach);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CoachViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Branches = await _api.GetBranchesAsync();
                return View(model);
            }

            var success = await _api.UpdateCoachAsync(id, model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Antrenör bilgileri güncellenirken API'de hata oluştu.");
                ViewBag.Branches = await _api.GetBranchesAsync();
                return View(model);
            }

            _logger.LogInformation("Antrenör bilgileri başarıyla güncellendi. ID: {Id}", id);
            TempData["Success"] = "Antrenör bilgileri başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _api.DeleteCoachAsync(id);
            if (!success)
            {
                TempData["Error"] = "Antrenör silinemedi. Atanmış aktif seans programı bulunuyor olabilir.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogInformation("Antrenör başarıyla silindi. ID: {Id}", id);
            TempData["Success"] = "Antrenör kaydı başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
