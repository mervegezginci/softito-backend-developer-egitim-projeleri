using kurs_javascriptcodefirstproje.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace kurs_javascriptcodefirstproje.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext context;

        public AdminController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            ViewBag.CategoryCount = context.Categories.Count();
            ViewBag.CourseCount = context.Courses.Count();
            ViewBag.EnrollmentCount = context.Enrollments.Count();

            // 📊 Kategoriye göre kurs sayısı
            var categoryStats = context.Categories
                .Select(c => new
                {
                    CategoryName = c.CategoryName,
                    CourseCount = c.Courses.Count()
                })
                .ToList();

            // 📊 Kurslara göre başvuru sayısı
            var courseStats = context.Courses
                .Select(c => new
                {
                    CourseName = c.CourseName,
                    EnrollmentCount = c.Enrollments.Count()
                })
                .ToList();

            ViewBag.CategoryNames = categoryStats.Select(x => x.CategoryName).ToList();
            ViewBag.CategoryCounts = categoryStats.Select(x => x.CourseCount).ToList();

            ViewBag.CourseNames = courseStats.Select(x => x.CourseName).ToList();
            ViewBag.EnrollmentCounts = courseStats.Select(x => x.EnrollmentCount).ToList();

            return View();
        }


        public JsonResult GetReport(string type)
        {
            switch (type)
            {
                case "MostEnrollment":

                    var mostEnrollment = context.Courses
                        .OrderByDescending(x => x.Enrollments.Count)
                        .Select(x => new
                        {
                            Name = x.CourseName,
                            Value = x.Enrollments.Count
                        })
                        .FirstOrDefault();

                    return Json(mostEnrollment);

                case "MostExpensive":

                    var expensive = context.Courses
                        .OrderByDescending(x => x.Price)
                        .Select(x => new
                        {
                            Name = x.CourseName,
                            Value = x.Price
                        })
                        .FirstOrDefault();

                    return Json(expensive);

                case "Cheapest":

                    var cheap = context.Courses
                        .OrderBy(x => x.Price)
                        .Select(x => new
                        {
                            Name = x.CourseName,
                            Value = x.Price
                        })
                        .FirstOrDefault();

                    return Json(cheap);

                case "EnrollmentCount":

                    return Json(new
                    {
                        Name = "Toplam Başvuru",
                        Value = context.Enrollments.Count()
                    });

                case "LastCourses":

                    var lastCourses = context.Courses
                        .OrderByDescending(x => x.Id)
                        .Take(5)
                        .Select(x => new
                        {
                            Name = x.CourseName,
                            Value = x.Price
                        })
                        .ToList();

                    return Json(lastCourses);

                case "MostCategory":

                    var category = context.Categories
                        .OrderByDescending(x => x.Courses.Count)
                        .Select(x => new
                        {
                            Name = x.CategoryName,
                            Value = x.Courses.Count
                        })
                        .FirstOrDefault();

                    return Json(category);

                case "Over40Hours":

                    var courses = context.Courses
                        .Where(x => x.Duration > 40)
                        .Select(x => new
                        {
                            Name = x.CourseName,
                            Value = x.Duration
                        })
                        .ToList();

                    return Json(courses);

                case "CategoryCourseCount":

                    var categories = context.Categories
                        .Select(x => new
                        {
                            Name = x.CategoryName,
                            Value = x.Courses.Count()
                        })
                        .ToList();

                    return Json(categories);

                default:

                    return Json(null);
            }
        }
    }
}
