using kuafor_ORMproje.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace kuafor_ORMproje.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ReportController> _logger;

        private const string CacheKeyPrefix = "ReportData_";

        public ReportController(
            ApplicationDbContext context,
            IMemoryCache cache,
            ILogger<ReportController> _logger)
        {
            _context = context;
            _cache = cache;
            this._logger = _logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Raporlama sayfasına erişildi. Tarih: {Time}", DateTime.Now);

            // 1. Caching of Revenue by Service
            string serviceCacheKey = CacheKeyPrefix + "RevenueByService";
            if (!_cache.TryGetValue(serviceCacheKey, out List<ServiceRevenueViewModel> serviceRevenue))
            {
                _logger.LogInformation("Hizmet bazlı gelir raporu önbellekte bulunamadı, veritabanından çekiliyor.");
                
                var queryResult = await _context.Payments
                    .Include(p => p.Appointment)
                        .ThenInclude(a => a!.Service)
                    .Where(p => p.PaymentStatus && p.Appointment != null && p.Appointment.Service != null)
                    .ToListAsync(); // Read into memory first to avoid SQLite double conversion in group

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

                var queryResult = await _context.Appointments
                    .Include(a => a.Employee)
                    .Where(a => a.Employee != null)
                    .ToListAsync();

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

            // 3. System Logs Mock (to show logging actions to administrator)
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
