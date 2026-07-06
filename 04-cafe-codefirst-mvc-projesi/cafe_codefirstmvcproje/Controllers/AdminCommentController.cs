using cafe_codefirstmvcproje.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cafe_codefirstmvcproje.Controllers
{
    public class AdminCommentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminCommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search)
        {
            var query = _context.Comments
                .Include(x => x.Product)
                .Include(x => x.Customer)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x =>
                    x.Text.Contains(search) ||
                    x.Product.ProductName.Contains(search) ||
                    x.Customer.FullName.Contains(search)
                );
            }

            return View(query.ToList());
        }
        public IActionResult Delete(int id)
        {
            var comment = _context.Comments.Find(id);

            if (comment == null)
                return NotFound();

            _context.Comments.Remove(comment);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Update(int id)
        {
            var comment = _context.Comments
                .Include(x => x.Customer)
                .Include(x => x.Product)
                .FirstOrDefault(x => x.Id == id);

            if (comment == null)
                return NotFound();

            return View(comment);
        }

        [HttpPost]
        public IActionResult Update(Comment model)
        {
            var comment = _context.Comments.Find(model.Id);

            if (comment == null)
                return NotFound();

            comment.Text = model.Text;
            comment.IsApproved = model.IsApproved;
            comment.CustomerId = model.CustomerId;
            comment.ProductId = model.ProductId;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}