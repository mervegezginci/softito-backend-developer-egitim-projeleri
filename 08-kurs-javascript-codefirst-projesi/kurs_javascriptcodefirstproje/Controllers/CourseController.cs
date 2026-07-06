using Microsoft.AspNetCore.Mvc;
using kurs_javascriptcodefirstproje.Models;

namespace kurs_javascriptcodefirstproje.Controllers
{
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext context;

        public CourseController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public JsonResult CourseList(string search = "")
        {
            var data = context.Courses.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x =>
                    x.CourseName.Contains(search) ||
                    x.Description.Contains(search));
            }

            var result = data.Select(x => new
            {
                x.Id,
                x.CourseName,
                x.Description,
                x.Price,
                x.Duration,
                x.ImageUrl,
                CategoryName = x.Category.CategoryName
            }).ToList();

            return Json(result);
        }

        public JsonResult CategoryList()
        {
            var data = context.Categories
                .Select(x => new
                {
                    x.Id,
                    x.CategoryName
                })
                .ToList();

            return Json(data);
        }

        // ADD
        [HttpPost]
        public JsonResult AddCourse(Course course)
        {
            context.Courses.Add(course);
            context.SaveChanges();

            return Json("Course saved");
        }

        // EDIT
        public JsonResult Edit(int id)
        {
            var data = context.Courses
                .SingleOrDefault(x => x.Id == id);

            return Json(data);
        }

        // UPDATE
        [HttpPost]
        public JsonResult Update(Course course)
        {
            context.Courses.Update(course);
            context.SaveChanges();

            return Json("Course updated");
        }

        // DELETE
        public JsonResult Delete(int id)
        {
            var data = context.Courses
                .SingleOrDefault(x => x.Id == id);

            if (data != null)
            {
                context.Courses.Remove(data);
                context.SaveChanges();
            }

            return Json("Course deleted");
        }
    }
}