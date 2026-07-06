using kutuphane_apiproje.Data;
using kutuphane_apiproje.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace kutuphane_apiproje.Controllers
{
    public class AuthorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Listeleme
        public async Task<IActionResult> Index(string? search)
        {
            ViewBag.Search = search;
            var authors = _context.Authors.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                authors = authors.Where(a => a.FullName.Contains(search) || a.Country.Contains(search));
            return View(await authors.ToListAsync());
        }

        // Ekleme Sayfası
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Ekleme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Author author)
        {
            if (ModelState.IsValid)
            {
                _context.Authors.Add(author);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(author);
        }

        // Güncelleme Sayfası
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var author = await _context.Authors.FindAsync(id);

            if (author == null)
                return NotFound();

            return View(author);
        }

        // Güncelleme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Author author)
        {
            if (id != author.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(author);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _context.Authors.FindAsync(id);

            if (author != null)
            {
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}