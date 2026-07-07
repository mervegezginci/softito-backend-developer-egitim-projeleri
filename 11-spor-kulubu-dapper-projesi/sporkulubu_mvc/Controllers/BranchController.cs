using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using sporkulubu_mvc.Models;
using sporkulubu_mvc.Services;

namespace sporkulubu_mvc.Controllers
{
    [Authorize]
    public class BranchController : Controller
    {
        private readonly ApiService _api;
        private readonly IMemoryCache _cache;
        private readonly ILogger<BranchController> _logger;
        private const string CacheKey = "BranchesList";

        public BranchController(ApiService api, IMemoryCache cache, ILogger<BranchController> logger)
        {
            _api = api;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Spor branşları listesi görüntülendi (Cache kontrol ediliyor).");

            if (!_cache.TryGetValue(CacheKey, out IEnumerable<BranchViewModel>? branches))
            {
                _logger.LogWarning("Spor branşları cache'de bulunamadı. API'den çekiliyor...");
                branches = await _api.GetBranchesAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(CacheKey, branches, cacheEntryOptions);
            }
            else
            {
                _logger.LogInformation("Spor branşları cache'den çekildi.");
            }

            return View(branches);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(BranchViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _api.CreateBranchAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Branş eklenirken API'de hata oluştu.");
                return View(model);
            }

            _logger.LogInformation("Yeni branş başarıyla eklendi. Cache temizleniyor.");
            _cache.Remove(CacheKey);

            TempData["Success"] = "Spor branşı başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var branch = await _api.GetBranchByIdAsync(id);
            if (branch == null) return NotFound();
            return View(branch);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, BranchViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _api.UpdateBranchAsync(id, model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Branş güncellenirken API'de hata oluştu.");
                return View(model);
            }

            _logger.LogInformation("Branş güncellemesi başarılı. Cache temizleniyor.");
            _cache.Remove(CacheKey);

            TempData["Success"] = "Spor branşı başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _api.DeleteBranchAsync(id);
            if (!success)
            {
                TempData["Error"] = "Spor branşı silinemedi. Bu branşa atanmış antrenörler veya antrenmanlar bulunuyor olabilir.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogInformation("Branş silme işlemi başarılı. Cache temizleniyor.");
            _cache.Remove(CacheKey);

            TempData["Success"] = "Spor branşı başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
