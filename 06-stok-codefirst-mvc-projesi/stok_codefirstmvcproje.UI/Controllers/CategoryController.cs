using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using stok_codefirstmvcproje.data.Data;
using stok_codefirstmvcproje.model;
using System;

namespace stok_codefirstmvcproje.UI.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search)
        {
            var data = _context.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                data = data.Where(x => x.CategoryName.Contains(search));

            return View(await data.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            try
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            return View(await _context.Categories.FindAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            _context.Update(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}