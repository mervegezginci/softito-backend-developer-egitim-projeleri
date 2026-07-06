using kutuphane_apiproje.Data;
using kutuphane_apiproje.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace kutuphane_apiproje.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class BorrowingController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public BorrowingController(ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> MyBooks()
    {
        var user = await _userManager.GetUserAsync(User);

        var books = await _context.Borrowings
            .Include(x => x.Book)
            .ThenInclude(x => x.Author)
            .Where(x => x.UserId == user.Id)
            .OrderByDescending(x => x.BorrowDate)
            .ToListAsync();

        return View(books);
    }

    // Kullanıcının kendi kitabını iade etmesi
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReturnMyBook(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        var borrowing = await _context.Borrowings
            .Include(b => b.Book)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == user.Id);

        if (borrowing == null)
            return NotFound();

        borrowing.IsReturned = true;
        borrowing.ReturnDate = DateTime.Now;

        if (borrowing.Book != null)
        {
            borrowing.Book.IsAvailable = true;
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = "Kitabı başarıyla iade ettiniz!";
        return RedirectToAction(nameof(MyBooks));
    }


    // Tüm ödünçler
    public async Task<IActionResult> Index(string? search)
    {
        ViewBag.Search = search;
        var borrowings = _context.Borrowings
            .Include(b => b.Book)
            .Include(b => b.User)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            borrowings = borrowings.Where(b => b.Book.Title.Contains(search) || b.User.Name.Contains(search));
        return View(await borrowings.ToListAsync());
    }

    // Aktif ödünçler
    public async Task<IActionResult> Active()
    {
        var borrowings = await _context.Borrowings
            .Include(b => b.Book)
            .Include(b => b.User)
            .Where(b => !b.IsReturned)
            .ToListAsync();

        return View(borrowings);
    }

    // Ödünç verme sayfası
    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Books = new SelectList(
            _context.Books.Where(x => x.IsAvailable),
            "Id",
            "Title");

        ViewBag.Users = new SelectList(
            _context.Users,
            "Id",
            "UserName");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Borrowing borrowing)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Books = new SelectList(_context.Books.Where(x => x.IsAvailable), "Id", "Title");
            ViewBag.Users = new SelectList(_context.Users, "Id", "UserName");
            return View(borrowing);
        }

        // Giriş yapan kullanıcının Identity Id'sini al
        borrowing.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(borrowing.UserId))
        {
            return RedirectToAction("Login", "Account");
        }

        borrowing.BorrowDate = DateTime.Now;
        borrowing.DueDate = DateTime.Now.AddDays(15);
        borrowing.IsReturned = false;

        _context.Borrowings.Add(borrowing);

        var book = await _context.Books.FindAsync(borrowing.BookId);

        if (book != null)
        {
            book.IsAvailable = false;
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = "Kitap ödünç alma talebiniz başarıyla oluşturuldu.";

        return RedirectToAction("Details", "Book", new { id = borrowing.BookId });
    }

    // Detay
    public async Task<IActionResult> Details(int id)
    {
        var borrowing = await _context.Borrowings
            .Include(b => b.Book)
            .Include(b => b.User)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (borrowing == null)
            return NotFound();

        return View(borrowing);
    }

    // İade işlemi
    public async Task<IActionResult> ReturnBook(int id)
    {
        var borrowing = await _context.Borrowings
            .Include(b => b.Book)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (borrowing == null)
            return NotFound();

        borrowing.IsReturned = true;
        borrowing.ReturnDate = DateTime.Now;

        if (borrowing.Book != null)
        {
            borrowing.Book.IsAvailable = true;
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // Silme
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var borrowing = await _context.Borrowings
            .Include(b => b.Book)
            .Include(b => b.User)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (borrowing == null)
            return NotFound();

        return View(borrowing);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var borrowing = await _context.Borrowings.FindAsync(id);

        if (borrowing != null)
        {
            _context.Borrowings.Remove(borrowing);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

}