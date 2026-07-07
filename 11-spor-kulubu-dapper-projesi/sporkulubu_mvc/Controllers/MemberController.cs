using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using sporkulubu_mvc.Models;
using sporkulubu_mvc.Services;

namespace sporkulubu_mvc.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly ApiService _api;
        private readonly ILogger<MemberController> _logger;

        public MemberController(ApiService api, ILogger<MemberController> logger)
        {
            _api = api;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? keyword, bool? isActive)
        {
            _logger.LogInformation("Üye listesi görüntülendi. Filtreler: Keyword={Keyword}, Durum={IsActive}", keyword, isActive);
            var members = await _api.GetMembersAsync(keyword, isActive);
            ViewBag.Keyword = keyword;
            ViewBag.IsActive = isActive;
            return View(members);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(MemberViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _api.CreateMemberAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Üye eklenirken API'de hata oluştu. E-posta adresi kullanımda olabilir.");
                return View(model);
            }

            _logger.LogInformation("Yeni üye başarıyla kaydedildi.");
            TempData["Success"] = "Üye başarıyla kaydedildi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var member = await _api.GetMemberByIdAsync(id);
            if (member == null) return NotFound();
            return View(member);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, MemberViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _api.UpdateMemberAsync(id, model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Üye bilgileri güncellenirken API'de hata oluştu.");
                return View(model);
            }

            _logger.LogInformation("Üye bilgileri güncellendi. ID: {Id}", id);
            TempData["Success"] = "Üye bilgileri başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _api.DeleteMemberAsync(id);
            if (!success)
            {
                TempData["Error"] = "Üye kaydı silinemedi.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogInformation("Üye kaydı silindi. ID: {Id}", id);
            TempData["Success"] = "Üye kaydı başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ExportPdf()
        {
            var pdfBytes = await _api.DownloadPdfAsync();
            if (pdfBytes == null)
            {
                TempData["Error"] = "PDF raporu indirilirken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
            return File(pdfBytes, "application/pdf", $"Spor_Kulubu_Uye_Listesi_{DateTime.Now:yyyyMMdd}.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel()
        {
            var excelBytes = await _api.DownloadExcelAsync();
            if (excelBytes == null)
            {
                TempData["Error"] = "Excel raporu indirilirken hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Spor_Kulubu_Uye_Listesi_{DateTime.Now:yyyyMMdd}.xlsx");
        }
    }
}
