using Microsoft.AspNetCore.Mvc;
using kurs_javascriptcodefirstproje.Models;

namespace kurs_javascriptcodefirstproje.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext context;

        public CategoryController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public JsonResult CategoryList(string search = "")
        {
            var data = context.Categories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.CategoryName.Contains(search));
            }

            return Json(data.ToList());
        }

        [HttpPost]
        public JsonResult AddCategory(Category category)
        {
            var cat = new Category()
            {
                CategoryName = category.CategoryName
            };

            context.Categories.Add(cat);
            context.SaveChanges();

            return new JsonResult("Category saved");
        }

        public JsonResult Edit(int id)
        {
            var data = context.Categories
                .Where(x => x.Id == id)
                .SingleOrDefault();

            return new JsonResult(data);
        }

        [HttpPost]
        public JsonResult Update(Category category)
        {
            context.Categories.Update(category);
            context.SaveChanges();

            return new JsonResult("Category updated");
        }

        public JsonResult Delete(int id)
        {
            var data = context.Categories
                .Where(x => x.Id == id)
                .SingleOrDefault();

            context.Categories.Remove(data);
            context.SaveChanges();

            return new JsonResult("Category deleted");
        }
    }
}