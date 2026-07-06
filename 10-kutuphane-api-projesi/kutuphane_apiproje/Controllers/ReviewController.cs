using kutuphane_apiproje.Data;
using kutuphane_apiproje.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace kutuphane_apiproje.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Tüm yorumlar
        public async Task<IActionResult> Index(string? search)
        {
            ViewBag.Search = search;
            var reviews = _context.Reviews
                .Include(r => r.Book)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedDate)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                reviews = reviews.Where(r => r.Comment.Contains(search) || r.Book.Title.Contains(search));
            return View(await reviews.ToListAsync());
        }

        // Detay
        public async Task<IActionResult> Details(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Book)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
                return NotFound();

            return View(review);
        }

        // Yeni yorum sayfası
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Books = new SelectList(_context.Books, "Id", "Title");
            ViewBag.Users = new SelectList(_context.Users, "Id", "UserName");

            return View();
        }

        // Yorum ekleme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Review review)
        {
            if (ModelState.IsValid)
            {
                review.CreatedDate = DateTime.Now;

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Books = new SelectList(_context.Books, "Id", "Title", review.BookId);
            ViewBag.Users = new SelectList(_context.Users, "Id", "UserName", review.UserId);

            return View(review);
        }

        // Düzenleme sayfası
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
                return NotFound();

            ViewBag.Books = new SelectList(_context.Books, "Id", "Title", review.BookId);
            ViewBag.Users = new SelectList(_context.Users, "Id", "UserName", review.UserId);

            return View(review);
        }

        // Güncelleme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Review review)
        {
            if (id != review.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(review);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Books = new SelectList(_context.Books, "Id", "Title", review.BookId);
            ViewBag.Users = new SelectList(_context.Users, "Id", "UserName", review.UserId);

            return View(review);
        }

        // Silme sayfası
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Book)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
                return NotFound();

            return View(review);
        }

        // Silme işlemi
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Kitaba göre yorumlar
        public async Task<IActionResult> BookReviews(int bookId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.BookId == bookId)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();

            return View(reviews);
        }
    }
}