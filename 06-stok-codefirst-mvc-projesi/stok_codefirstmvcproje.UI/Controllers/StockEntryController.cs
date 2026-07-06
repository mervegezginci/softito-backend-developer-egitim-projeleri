using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using stok_codefirstmvcproje.data.Data;
using stok_codefirstmvcproje.model;

namespace stok_codefirstmvcproje.UI.Controllers
{
    public class StockEntryController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public StockEntryController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        public IActionResult Index()
        {
            var values = dbContext.StockEntries
                .Include(x => x.Product)
                .Include(x => x.Supplier)
                .OrderByDescending(x => x.EntryDate)
                .ToList();

            return View(values);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Products = dbContext.Products.ToList();
            ViewBag.Suppliers = dbContext.Suppliers.ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Create(StockEntry stockEntry)
        {
            var product = dbContext.Products
                .FirstOrDefault(x => x.Id == stockEntry.ProductId);

            if (product == null)
            {
                return NotFound();
            }

            product.StockQty += stockEntry.Quantity;

            stockEntry.EntryDate = DateTime.Now;

            dbContext.StockEntries.Add(stockEntry);

            dbContext.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}