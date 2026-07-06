using kutuphane_apiproje.Data;
using kutuphane_apiproje.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace kutuphane_apiproje.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Listeleme
        public async Task<IActionResult> Index(string? search)
        {
            ViewBag.Search = search;
            var books = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                books = books.Where(b => b.Title.Contains(search) ||
                    b.Author.FullName.Contains(search) ||
                    b.Category.CategoryName.Contains(search));
            return View(await books.ToListAsync());
        }

        // Detay
        public async Task<IActionResult> Details(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            return View(book);
        }

        // Ekleme Sayfası
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Authors = new SelectList(_context.Authors, "Id", "FullName");
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "CategoryName");

            return View();
        }

        // Ekleme İşlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Authors = new SelectList(_context.Authors, "Id", "FullName", book.AuthorId);
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "CategoryName", book.CategoryId);

            return View(book);
        }

        // Güncelleme Sayfası
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
                return NotFound();

            ViewBag.Authors = new SelectList(_context.Authors, "Id", "FullName", book.AuthorId);
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "CategoryName", book.CategoryId);

            return View(book);
        }

        // Güncelleme İşlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book book)
        {
            if (id != book.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(book);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Authors = new SelectList(_context.Authors, "Id", "FullName", book.AuthorId);
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "CategoryName", book.CategoryId);

            return View(book);
        }

        // Silme İşlemi
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}