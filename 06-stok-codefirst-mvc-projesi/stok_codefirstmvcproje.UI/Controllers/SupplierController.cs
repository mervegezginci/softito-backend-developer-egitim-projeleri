using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using stok_codefirstmvcproje.data.Data;
using stok_codefirstmvcproje.model;
using System;

namespace stok_codefirstmvcproje.UI.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupplierController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search)
        {
            var data = _context.Suppliers.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                data = data.Where(x => x.CompanyName.Contains(search));

            return View(await data.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Supplier supplier)
        {
            if (!ModelState.IsValid)
                return View(supplier);

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Supplier supplier)
        {
            if (!ModelState.IsValid)
                return View(supplier);

            _context.Suppliers.Update(supplier);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            return View(await _context.Suppliers.FindAsync(id));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);

            _context.Remove(supplier);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}