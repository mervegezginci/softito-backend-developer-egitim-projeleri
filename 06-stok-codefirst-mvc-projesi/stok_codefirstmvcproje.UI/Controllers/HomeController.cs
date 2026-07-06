using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using stok_codefirstmvcproje.data.Data;
using stok_codefirstmvcproje.model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace stok_codefirstmvcproje.UI.Controllers
{
    public class ReportItem
    {
        public string Bilgi { get; set; } = "";
        public string Deger { get; set; } = "";
    }

    public class HomeController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public HomeController(ApplicationDbContext context)
        {
            this.dbContext = context;
        }

        public IActionResult Index()
        {
            // Ana Metrikler
            ViewBag.TotalProduct = dbContext.Products.Count();
            ViewBag.TotalCategory = dbContext.Categories.Count();
            ViewBag.TotalSupplier = dbContext.Suppliers.Count();
            ViewBag.TotalStock = dbContext.Products.Sum(x => (int?)x.StockQty) ?? 0;
            ViewBag.TotalStockValue = dbContext.Products.Sum(x => (decimal?)(x.StockQty * x.UnitPrice)) ?? 0;

            // Kategoriye Göre Toplam Envanter Değeri (Grafik 1 için)
            ViewBag.CategoryStockValues = dbContext.Categories
                .Select(c => new
                {
                    CategoryName = c.CategoryName,
                    TotalValue = dbContext.Products
                        .Where(p => p.CategoryId == c.Id)
                        .Sum(p => (decimal?)(p.StockQty * p.UnitPrice)) ?? 0
                }).ToList();

            // En Yüksek Stoklu Ürünler (Grafik 2 için)
            ViewBag.TopStockProducts = dbContext.Products
                .OrderByDescending(p => p.StockQty)
                .Take(5)
                .Select(p => new
                {
                    ProductName = p.Name,
                    Stock = p.StockQty
                }).ToList();

            // Toplam Giriş vs Toplam Sipariş (Çıkış) Miktarları (Grafik 3 için)
            ViewBag.TotalStockIn = dbContext.StockEntries.Sum(x => (int?)x.Quantity) ?? 0;
            ViewBag.TotalStockOut = dbContext.Orders.Sum(x => (int?)x.Quantity) ?? 0;

            // Kritik Stok Seviyesindeki Ürünler (Stok adeti <= 15 olanlar)
            ViewBag.CriticalStockItems = dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => p.StockQty <= 15)
                .OrderBy(p => p.StockQty)
                .Take(10)
                .ToList();

            // Ürün Detayları (Son 10 Ürün)
            ViewBag.ProductDetails = (
                from p in dbContext.Products
                join c in dbContext.Categories on p.CategoryId equals c.Id
                join s in dbContext.Suppliers on p.SupplierId equals s.Id
                select new
                {
                    Product = p.Name,
                    Category = c.CategoryName,
                    Supplier = s.CompanyName,
                    Stock = p.StockQty,
                    Price = p.UnitPrice
                }).Take(10).ToList();

            return View();
        }

        [HttpGet]
        public IActionResult GetReportData(string reportType)
        {
            var selectedReportType = reportType ?? "kritik_stok_seviyeleri";
            var reportData = new List<ReportItem>();

            switch (selectedReportType)
            {
                case "kritik_stok_seviyeleri":
                    reportData = dbContext.Products
                        .Where(p => p.StockQty <= 15)
                        .OrderBy(p => p.StockQty)
                        .Select(p => new ReportItem
                        {
                            Bilgi = p.Name,
                            Deger = p.StockQty + " Adet"
                        }).ToList();
                    break;
                case "kategori_stok_degerleri":
                    reportData = dbContext.Categories
                        .Select(c => new ReportItem
                        {
                            Bilgi = c.CategoryName,
                            Deger = (dbContext.Products.Where(p => p.CategoryId == c.Id).Sum(p => (decimal?)(p.StockQty * p.UnitPrice)) ?? 0).ToString("N2") + " ₺"
                        }).ToList();
                    break;
                case "tedarikci_urun_sayilari":
                    reportData = dbContext.Suppliers
                        .Select(s => new ReportItem
                        {
                            Bilgi = s.CompanyName,
                            Deger = dbContext.Products.Count(p => p.SupplierId == s.Id) + " Çeşit Ürün"
                        }).ToList();
                    break;
                case "en_yuksek_stoklar":
                    reportData = dbContext.Products
                        .OrderByDescending(p => p.StockQty)
                        .Take(5)
                        .Select(p => new ReportItem
                        {
                            Bilgi = p.Name,
                            Deger = p.StockQty.ToString("N0") + " Adet"
                        }).ToList();
                    break;
                case "toplam_giris_miktarlari":
                    reportData = dbContext.Products
                        .Select(p => new ReportItem
                        {
                            Bilgi = p.Name,
                            Deger = (dbContext.StockEntries.Where(se => se.ProductId == p.Id).Sum(se => (int?)se.Quantity) ?? 0).ToString("N0") + " Adet Giriş"
                        }).ToList();
                    break;
            }

            return Json(reportData);
        }
    }
}
