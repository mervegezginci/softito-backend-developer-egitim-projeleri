using kutuphane_apiproje.Data;
using kutuphane_apiproje.Models;
using Microsoft.AspNetCore.Mvc;

namespace kutuphane_apiproje.Controllers
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
            DashboardViewModel model = new DashboardViewModel()
            {
                TotalBooks = _context.Books.Count(),
                TotalAuthors = _context.Authors.Count(),
                TotalCategories = _context.Categories.Count(),
                TotalUsers = _context.Users.Count(),
                ActiveBorrowings = _context.Borrowings.Count(x => !x.IsReturned),
                ReturnedBooks = _context.Borrowings.Count(x => x.IsReturned),
                TotalReviews = _context.Reviews.Count(),
                AvailableBooks = _context.Books.Count(x => x.IsAvailable),
                UnavailableBooks = _context.Books.Count(x => !x.IsAvailable)
            };

            return View(model);
        }


        [HttpGet]
        public JsonResult GetReport(int reportType)
        {
            switch (reportType)
            {
                // 1 - En Çok Ödünç Alınan Kitaplar
                case 1:
                    return Json(
                        _context.Borrowings
                        .Join(_context.Books,
                            b => b.BookId,
                            k => k.Id,
                            (b, k) => new
                            {
                                k.Title
                            })
                        .GroupBy(x => x.Title)
                        .Select(x => new
                        {
                            Bilgi = x.Key,
                            Deger = x.Count()
                        })
                        .OrderByDescending(x => x.Deger)
                        .Take(10)
                        .ToList()
                    );

                // 2 - En Son Ödünç Alınan 5 Kitap
                case 2:
                    return Json(
                        _context.Borrowings
                        .OrderByDescending(x => x.BorrowDate)
                        .Take(5)
                        .Join(_context.Books,
                            b => b.BookId,
                            k => k.Id,
                            (b, k) => new
                            {
                                Bilgi = k.Title,
                                Deger = b.BorrowDate.ToString("dd.MM.yyyy")
                            })
                        .ToList()
                    );

                // 3 - En Çok Kitabı Olan Yazarlar
                case 3:
                    return Json(
                        _context.Authors
                        .Select(a => new
                        {
                            Bilgi = a.FullName,
                            Deger = _context.Books.Count(x => x.AuthorId == a.Id)
                        })
                        .OrderByDescending(x => x.Deger)
                        .ToList()
                    );

                // 4 - Kategori Bazlı Kitap Sayıları
                case 4:
                    return Json(
                        _context.Categories
                        .Select(c => new
                        {
                            Bilgi = c.CategoryName,
                            Deger = _context.Books.Count(x => x.CategoryId == c.Id)
                        })
                        .OrderByDescending(x => x.Deger)
                        .ToList()
                    );

                // 5 - Ödünçte Olan Kitaplar
                case 5:
                    return Json(
                        _context.Books
                        .Where(x => !x.IsAvailable)
                        .Select(x => new
                        {
                            Bilgi = x.Title,
                            Deger = "Ödünçte"
                        })
                        .ToList()
                    );

                // 6 - Müsait Kitaplar
                case 6:
                    return Json(
                        _context.Books
                        .Where(x => x.IsAvailable)
                        .Select(x => new
                        {
                            Bilgi = x.Title,
                            Deger = "Müsait"
                        })
                        .ToList()
                    );

                // 7 - En Çok Yorum Alan Kitaplar
                case 7:
                    return Json(
                        _context.Reviews
                        .Join(_context.Books,
                            r => r.BookId,
                            b => b.Id,
                            (r, b) => new
                            {
                                b.Title
                            })
                        .GroupBy(x => x.Title)
                        .Select(x => new
                        {
                            Bilgi = x.Key,
                            Deger = x.Count()
                        })
                        .OrderByDescending(x => x.Deger)
                        .ToList()
                    );

                // 8 - Son Eklenen Kitaplar
                case 8:
                    return Json(
                        _context.Books
                        .OrderByDescending(x => x.Id)
                        .Take(10)
                        .Select(x => new
                        {
                            Bilgi = x.Title,
                            Deger = x.PublishYear
                        })
                        .ToList()
                    );

                // 9 - En Eski Yayınlanan Kitaplar
                case 9:
                    return Json(
                        _context.Books
                        .OrderBy(x => x.PublishYear)
                        .Take(10)
                        .Select(x => new
                        {
                            Bilgi = x.Title,
                            Deger = x.PublishYear
                        })
                        .ToList()
                    );

                // 10 - En Aktif Okuyucular
                case 10:
                    return Json(
                        _context.Borrowings
                        .Join(_context.Users,
                            b => b.UserId,
                            u => u.Id,
                            (b, u) => new
                            {
                                u.UserName
                            })
                        .GroupBy(x => x.UserName)
                        .Select(x => new
                        {
                            Bilgi = x.Key,
                            Deger = x.Count()
                        })
                        .OrderByDescending(x => x.Deger)
                        .ToList()
                    );

                default:
                    return Json(new List<object>());
            }
        }
    }
}