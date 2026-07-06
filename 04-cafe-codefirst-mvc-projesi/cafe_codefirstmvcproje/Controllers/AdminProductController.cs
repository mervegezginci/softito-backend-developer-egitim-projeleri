using cafe_codefirstmvcproje.Models;
using Microsoft.AspNetCore.Mvc;

namespace cafe_codefirstmvcproje.Controllers
{
    public class AdminProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.ProductName.Contains(search));
            }

            return View(query.ToList());
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var value = _context.Products.Find(id);

            _context.Products.Remove(value);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Update(int id)
        {
            ViewBag.Categories = _context.Categories.ToList();

            var value = _context.Products.Find(id);

            return View(value);
        }

        [HttpPost]
        public IActionResult Update(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}