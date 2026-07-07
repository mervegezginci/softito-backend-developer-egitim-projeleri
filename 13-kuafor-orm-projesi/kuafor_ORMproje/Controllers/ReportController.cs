using kuafor_ORMproje.Data.Repository.IRepository;
using kuafor_ORMproje.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace kuafor_ORMproje.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ReportController> _logger;

        private const string CacheKeyPrefix = "ReportData_";

        public ReportController(
            IUnitOfWork unitOfWork,
            IMemoryCache cache,
            ILogger<ReportController> logger)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Raporlama sayfasına erişildi. Tarih: {Time}", DateTime.Now);

            // 1. Caching of Revenue by Service
            string serviceCacheKey = CacheKeyPrefix + "RevenueByService";
            if (!_cache.TryGetValue(serviceCacheKey, out List<ServiceRevenueViewModel> serviceRevenue))
            {
                _logger.LogInformation("Hizmet bazlı gelir raporu önbellekte bulunamadı, veritabanından çekiliyor.");
                
                var queryResult = _unitOfWork.Payment
                    .GetAll(p => p.PaymentStatus && p.Appointment != null && p.Appointment.Service != null, includeProperties: "Appointment,Appointment.Service")
                    .ToList();

                serviceRevenue = queryResult
                    .GroupBy(p => p.Appointment!.Service!.ServiceName)
                    .Select(g => new ServiceRevenueViewModel
                    {
                        ServiceName = g.Key,
                        TotalAmount = g.Sum(p => p.Amount),
                        Count = g.Count()
                    })
                    .ToList();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(serviceCacheKey, serviceRevenue, cacheEntryOptions);
            }

            // 2. Caching of Employee Performance
            string employeeCacheKey = CacheKeyPrefix + "EmployeePerformance";
            if (!_cache.TryGetValue(employeeCacheKey, out List<EmployeePerformanceViewModel> employeePerformance))
            {
                _logger.LogInformation("Çalışan performans raporu önbellekte bulunamadı, veritabanından çekiliyor.");

                var queryResult = _unitOfWork.Appointment
                    .GetAll(a => a.Employee != null, includeProperties: "Employee")
                    .ToList();

                employeePerformance = queryResult
                    .GroupBy(a => a.Employee!.FullName)
                    .Select(g => new EmployeePerformanceViewModel
                    {
                        EmployeeName = g.Key,
                        AppointmentCount = g.Count(),
                        CompletedCount = g.Count(a => a.Status == "Tamamlandı" || a.Status == "Onaylandı")
                    })
                    .ToList();

                _cache.Set(employeeCacheKey, employeePerformance, TimeSpan.FromMinutes(5));
            }

            // 3. Mock data log
            ViewBag.ServiceRevenue = serviceRevenue;
            ViewBag.EmployeePerformance = employeePerformance;

            return View();
        }

        // Action to clear report cache manually
        public IActionResult ClearCache()
        {
            _cache.Remove(CacheKeyPrefix + "RevenueByService");
            _cache.Remove(CacheKeyPrefix + "EmployeePerformance");
            _logger.LogWarning("Sistem raporları önbelleği yönetici tarafından temizlendi.");
            TempData["SuccessMessage"] = "Raporlama önbelleği başarıyla temizlendi.";
            return RedirectToAction(nameof(Index));
        }
    }

    public class ServiceRevenueViewModel
    {
        public string ServiceName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int Count { get; set; }
    }

    public class EmployeePerformanceViewModel
    {
        public string EmployeeName { get; set; } = string.Empty;
        public int AppointmentCount { get; set; }
        public int CompletedCount { get; set; }
    }
}
