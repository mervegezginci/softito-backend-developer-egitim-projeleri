using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using stok_codefirstmvcproje.data.Data;
using stok_codefirstmvcproje.model;
using System;

namespace stok_codefirstmvcproje.UI.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search)
        {
            var products = _context.Products
                .Include(x => x.Category)
                .Include(x => x.Supplier)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(x =>
                    x.Name.Contains(search) ||
                    x.Category.CategoryName.Contains(search) ||
                    x.Supplier.CompanyName.Contains(search));
            }

            return View(await products.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(
                _context.Categories,
                "Id",
                "CategoryName");

            ViewBag.Suppliers = new SelectList(
                _context.Suppliers,
                "Id",
                "CompanyName");

            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);

            ViewBag.Categories = new SelectList(
                _context.Categories,
                "Id",
                "CategoryName",
                product.CategoryId);

            ViewBag.Suppliers = new SelectList(
                _context.Suppliers,
                "Id",
                "CompanyName",
                product.SupplierId);

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            _context.Update(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}