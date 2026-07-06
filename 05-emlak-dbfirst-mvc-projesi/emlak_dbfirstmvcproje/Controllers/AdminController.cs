using Microsoft.AspNetCore.Mvc;
using emlak_dbfirstmvcproje.Models;
using System.Linq;
using System.Collections.Generic;

namespace emlak_dbfirstmvcproje.Controllers
{
    public class ReportItem
    {
        public string Bilgi { get; set; } = "";
        public string Deger { get; set; } = "";
    }

    public class AdminController : Controller
    {
        private readonly RealEstateDbContext _context;

        public AdminController(RealEstateDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var properties = _context.Properties;

            var last10 = properties
                .OrderByDescending(x => x.Id)
                .Take(10)
                .ToList();

            // KPI
            ViewBag.PropertyCount = properties.Count();
            ViewBag.CityCount = _context.Cities.Count();
            ViewBag.TotalValue = properties.Sum(x => x.Price);

            ViewBag.AveragePrice = properties.Any()
                ? properties.Average(x => x.Price)
                : 0;

            ViewBag.MaxPrice = properties.Any()
                ? properties.Max(x => x.Price)
                : 0;

            ViewBag.MaxPriceProperty = properties
                .OrderByDescending(x => x.Price)
                .Select(x => x.Title)
                .FirstOrDefault();

            // Chart data (Last 10 prices)
            ViewBag.LastTitles = last10.Select(x => x.Title).ToList();
            ViewBag.LastPrices = last10.Select(x => x.Price).ToList();

            // City-wise Property Distribution
            var cityStats = _context.Properties
                .GroupBy(x => x.City.Name)
                .Select(g => new {
                    CityName = g.Key ?? "Bilinmeyen",
                    Count = g.Count()
                }).ToList();
            ViewBag.CityNames = cityStats.Select(x => x.CityName).ToList();
            ViewBag.CityCounts = cityStats.Select(x => x.Count).ToList();

            // Property Type-wise Distribution (Satılık/Kiralık)
            var typeStats = _context.Properties
                .GroupBy(x => x.PropertyType.TypeName)
                .Select(g => new {
                    TypeName = g.Key ?? "Bilinmeyen",
                    Count = g.Count()
                }).ToList();
            ViewBag.TypeNames = typeStats.Select(x => x.TypeName).ToList();
            ViewBag.TypeCounts = typeStats.Select(x => x.Count).ToList();

            return View(last10);
        }

        [HttpGet]
        public IActionResult GetReportData(string reportType)
        {
            var selectedReportType = reportType ?? "sehir_ilan_sayilari";
            var reportData = new List<ReportItem>();

            switch (selectedReportType)
            {
                case "sehir_ilan_sayilari":
                    reportData = _context.Properties
                        .GroupBy(x => x.City.Name)
                        .Select(g => new ReportItem {
                            Bilgi = g.Key ?? "Bilinmeyen",
                            Deger = g.Count().ToString()
                        }).ToList();
                    break;
                case "emlak_tipi_ilan_sayilari":
                    reportData = _context.Properties
                        .GroupBy(x => x.PropertyType.TypeName)
                        .Select(g => new ReportItem {
                            Bilgi = g.Key ?? "Bilinmeyen",
                            Deger = g.Count().ToString()
                        }).ToList();
                    break;
                case "yuksek_fiyatli_ilanlar":
                    reportData = _context.Properties
                        .Where(x => x.Price > 5000000)
                        .Select(x => new ReportItem {
                            Bilgi = x.Title,
                            Deger = x.Price.ToString("N0") + " ₺"
                        }).ToList();
                    break;
                case "en_buyuk_ilanlar":
                    reportData = _context.Properties
                        .OrderByDescending(x => x.SquareMeter)
                        .Take(5)
                        .Select(x => new ReportItem {
                            Bilgi = x.Title,
                            Deger = x.SquareMeter.ToString() + " m²"
                        }).ToList();
                    break;
                case "danisman_ilan_sayilari":
                    reportData = _context.Properties
                        .GroupBy(x => x.Realtor.NameSurname)
                        .Select(g => new ReportItem {
                            Bilgi = g.Key ?? "Bilinmeyen",
                            Deger = g.Count().ToString()
                        }).ToList();
                    break;
            }

            return Json(reportData);
        }
    }
}