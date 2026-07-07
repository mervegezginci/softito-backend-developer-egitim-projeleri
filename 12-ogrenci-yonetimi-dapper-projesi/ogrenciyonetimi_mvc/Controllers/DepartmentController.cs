using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ogrenciyonetimi_mvc.Models;
using ogrenciyonetimi_mvc.Services;

namespace ogrenciyonetimi_mvc.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly ApiService _api;
        private readonly IMemoryCache _cache;
        private readonly ILogger<DepartmentController> _logger;
        private const string CacheKey = "DepartmentsList";

        public DepartmentController(ApiService api, IMemoryCache cache, ILogger<DepartmentController> logger)
        {
            _api = api;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Bölüm listesi görüntülendi (Cache kontrol ediliyor).");

            if (!_cache.TryGetValue(CacheKey, out IEnumerable<DepartmentViewModel>? departments))
            {
                _logger.LogWarning("Bölüm listesi cache'de bulunamadı. API'den çekiliyor...");
                departments = await _api.GetDepartmentsAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(CacheKey, departments, cacheEntryOptions);
            }
            else
            {
                _logger.LogInformation("Bölüm listesi cache'den çekildi.");
            }

            return View(departments);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(DepartmentViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _api.CreateDepartmentAsync(model);
            if (!success)
            {
                ModelState.AddModelError("", "Bölüm oluşturulurken API'de hata oluştu.");
                return View(model);
            }

            _logger.LogInformation("Yeni bölüm başarıyla eklendi. Cache temizleniyor.");
            _cache.Remove(CacheKey);

            TempData["Success"] = "Bölüm başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dept = await _api.GetDepartmentByIdAsync(id);
            if (dept == null) return NotFound();
            return View(dept);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, DepartmentViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _api.UpdateDepartmentAsync(id, model);
            if (!success)
            {
                ModelState.AddModelError("", "Bölüm güncellenirken API'de hata oluştu.");
                return View(model);
            }

            _logger.LogInformation("Bölüm güncellemesi başarılı. Cache temizleniyor.");
            _cache.Remove(CacheKey);

            TempData["Success"] = "Bölüm başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _api.DeleteDepartmentAsync(id);
            if (!success)
            {
                TempData["Error"] = "Bölüm silinemedi. Bu bölüme kayıtlı öğrenciler olabilir.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogInformation("Bölüm silme işlemi başarılı. Cache temizleniyor.");
            _cache.Remove(CacheKey);

            TempData["Success"] = "Bölüm başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
