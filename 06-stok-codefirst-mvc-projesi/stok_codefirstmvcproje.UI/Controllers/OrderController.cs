using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using stok_codefirstmvcproje.data.Data;
using stok_codefirstmvcproje.model;

namespace stok_codefirstmvcproje.UI.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public OrderController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        // Sipariş Listesi
        public IActionResult Index()
        {
            var values = dbContext.Orders
                .Include(x => x.Product)
                .ToList();

            return View(values);
        }

        // Sipariş Ekle Sayfası
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Products = dbContext.Products.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Order order)
        {
            var product = dbContext.Products
                .FirstOrDefault(x => x.Id == order.ProductId);

            if (product == null)
            {
                return NotFound();
            }

            if (product.StockQty < order.Quantity)
            {
                ViewBag.Error = "Yeterli stok yok.";
                ViewBag.Products = dbContext.Products.ToList();
                return View();
            }

            product.StockQty -= order.Quantity;

            order.OrderDate = DateTime.Now;

            dbContext.Orders.Add(order);

            dbContext.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}