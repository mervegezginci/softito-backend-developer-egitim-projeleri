using System.Diagnostics;
using kutuphane_apiproje.Data;
using kutuphane_apiproje.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace kutuphane_apiproje.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext context;

        public HomeController(ILogger<HomeController> logger,
                              ApplicationDbContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public IActionResult Index()
        {
            HomeViewModel model = new HomeViewModel();

            model.Books = context.Books
                .Include(x => x.Author)
                .Include(x => x.Category)
                .Select(book => new BookCardViewModel
                {
                    Id = book.Id,
                    Title = book.Title,
                    ImageUrl = book.ImageUrl,
                    AuthorId = book.AuthorId,
                    AuthorName = book.Author!.FullName,
                    CategoryName = book.Category!.CategoryName,
                    CategoryId = book.CategoryId,
                    IsAvailable = book.IsAvailable,

                    DueDate = context.Borrowings
                        .Where(b => b.BookId == book.Id && !b.IsReturned)
                        .Select(b => (DateTime?)b.DueDate)
                        .FirstOrDefault()
                })
                .ToList();

            model.Categories = context.Categories.ToList();
            model.Authors = context.Authors.ToList();

            model.Reviews = context.Reviews
                .Include(x => x.User)
                .Include(x => x.Book)
                .OrderByDescending(x => x.CreatedDate)
                .ToList(); model.UserCount = context.Users.Count();

            model.MostBorrowedBooks = context.Borrowings
              .Include(x => x.Book)
                  .ThenInclude(x => x.Author)
              .GroupBy(x => new
              {
                  x.BookId,
                  x.Book!.Title,
                  x.Book.ImageUrl,
                  Author = x.Book.Author!.FullName
              })
              .Select(x => new MostBorrowedBookViewModel
              {
                  BookId = x.Key.BookId,
                  Title = x.Key.Title,
                  ImageUrl = x.Key.ImageUrl,
                  AuthorName = x.Key.Author,
                  BorrowCount = x.Count()
              })
              .OrderByDescending(x => x.BorrowCount)
              .Take(3)
              .ToList();

            return View(model);
        }

        // ============================
        // DETAILS ACTION
        // ============================
        public IActionResult Details(int id)
        {
            var book = context.Books
                .Include(x => x.Author)
                .Include(x => x.Category)
                .FirstOrDefault(x => x.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}