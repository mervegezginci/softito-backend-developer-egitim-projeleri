using cafe_codefirstmvcproje.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cafe_codefirstmvcproje.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // 1. Kategori bazlı istatistikler (Grafik için verileri tek seferde alıyoruz)
            var categoryStats = _context.Categories
                .Select(c => new {
                    Name = c.CategoryName,
                    ProductCount = _context.Products.Count(p => p.CategoryId == c.Id),
                    AvgPrice = _context.Products.Where(p => p.CategoryId == c.Id).Average(p => (double?)p.Price) ?? 0
                }).ToList();

            ViewBag.CategoryNames = categoryStats.Select(x => x.Name).ToList();
            ViewBag.ProductCounts = categoryStats.Select(x => x.ProductCount).ToList();
            ViewBag.CategoryAvgPrices = categoryStats.Select(x => x.AvgPrice).ToList();

            // 2. Basit sayımlar
            ViewBag.ProductCount = _context.Products.Count();
            ViewBag.CategoryCount = _context.Categories.Count();
            ViewBag.CustomerCount = _context.Customers.Count();
            ViewBag.ApprovedCommentCount = _context.Comments.Count(x => x.IsApproved);
            ViewBag.MaxPrice = _context.Products.Any() ? _context.Products.Max(x => x.Price) : 0;
            ViewBag.MinPrice = _context.Products.Any() ? _context.Products.Min(x => x.Price) : 0;

            // 3. İsim bazlı istatistikler
            ViewBag.MostExpensiveProduct = _context.Products.OrderByDescending(x => x.Price).Select(x => x.ProductName).FirstOrDefault();
            ViewBag.CheapestProduct = _context.Products.OrderBy(x => x.Price).Select(x => x.ProductName).FirstOrDefault();

            // En çok yorum alan ürün
            ViewBag.MostCommentedProduct = _context.Comments
                .GroupBy(c => c.Product.ProductName)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault() ?? "Yorum Yok";

            // 4. Ortalama puan
            ViewBag.AverageRating = _context.Comments.Any(x => x.IsApproved)
                ? _context.Comments.Where(x => x.IsApproved).Average(x => (double)x.Rating).ToString("0.0")
                : "0";

            return View();
        }

        public IActionResult Reports()
        {
            var viewModel = new ReportsViewModel();

            // 1. Kategori bazlı özet
            viewModel.CategoryReports = _context.Categories
                .Select(c => new CategoryReportItem
                {
                    CategoryName = c.CategoryName,
                    ProductCount = _context.Products.Count(p => p.CategoryId == c.Id),
                    AvgPrice = _context.Products.Where(p => p.CategoryId == c.Id).Average(p => (double?)p.Price) ?? 0,
                    MinPrice = _context.Products.Where(p => p.CategoryId == c.Id).Min(p => (decimal?)p.Price) ?? 0,
                    MaxPrice = _context.Products.Where(p => p.CategoryId == c.Id).Max(p => (decimal?)p.Price) ?? 0
                }).ToList();

            // 2. Yorum puan dağılımı (1-5 yıldız arası)
            var ratingBreakdown = _context.Comments
                .GroupBy(c => c.Rating)
                .Select(g => new
                {
                    Rating = g.Key,
                    Count = g.Count()
                }).ToList();

            foreach (var rb in ratingBreakdown)
            {
                if (rb.Rating >= 1 && rb.Rating <= 5)
                {
                    viewModel.RatingCounts[rb.Rating - 1] = rb.Count;
                }
            }

            // 3. Ürün bazlı detaylar
            viewModel.ProductReports = _context.Products
                .Select(p => new ProductReportItem
                {
                    ProductId = p.Id,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    CategoryName = p.Category != null ? p.Category.CategoryName : "Kategorisiz",
                    IsPopular = p.IsPopular,
                    CommentCount = _context.Comments.Count(c => c.ProductId == p.Id),
                    AverageRating = _context.Comments.Where(c => c.ProductId == p.Id && c.IsApproved).Average(c => (double?)c.Rating) ?? 0
                }).ToList();

            // 4. Genel Sayılar
            viewModel.TotalProducts = _context.Products.Count();
            viewModel.TotalCategories = _context.Categories.Count();
            viewModel.TotalCustomers = _context.Customers.Count();
            viewModel.TotalComments = _context.Comments.Count();
            viewModel.ApprovedComments = _context.Comments.Count(c => c.IsApproved);
            viewModel.PendingComments = _context.Comments.Count(c => !c.IsApproved);

            return View(viewModel);
        }
    }
}