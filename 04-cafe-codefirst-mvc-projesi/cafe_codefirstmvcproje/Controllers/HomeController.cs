using System.Diagnostics;
using cafe_codefirstmvcproje.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cafe_codefirstmvcproje.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context,
                              ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public ActionResult Index()
        {
            HomeViewModel model = new HomeViewModel();

            model.Categories = _context.Categories.ToList();

            model.PopularProducts = _context.Products
                .Where(x => x.IsPopular)
                .Take(4)
                .ToList();

            model.Comments = _context.Comments
                .Include(x => x.Customer)
                .Include(x => x.Product)
                .Where(x => x.IsApproved)
                .ToList();

            return View(model);
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

        [HttpPost]
        public IActionResult SendComment(
            string fullname,
            string email,
            string text,
            int productId,
            int rating)
        {
            if (string.IsNullOrWhiteSpace(fullname) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(text))
            {
                TempData["Error"] = "Lütfen tüm alanlarý doldurun.";
                return RedirectToAction("Index");
            }

            Customer customer = new Customer()
            {
                FullName = fullname,
                Email = email
            };

            _context.Customers.Add(customer);
            _context.SaveChanges();

            Comment comment = new Comment()
            {
                Text = text,
                Rating = rating,
                CustomerId = customer.Id,
                ProductId = productId,
                IsApproved = false
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            TempData["Success"] = "Yorumunuz onay için gönderildi.";

            return RedirectToAction("Index");
        }
    }
}