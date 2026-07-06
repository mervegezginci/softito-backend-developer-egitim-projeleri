using cafe_codefirstmvcproje.Models;
using Microsoft.AspNetCore.Mvc;

namespace cafe_codefirstmvcproje.Controllers
{
    public class AdminCategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminCategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search)
        {
            var query = _context.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.CategoryName.Contains(search));
            }

            return View(query.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Update(int id)
        {
            var value = _context.Categories.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult Update(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        public IActionResult Delete(int id)
        {
            var value = _context.Categories.Find(id);

            _context.Categories.Remove(value);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}