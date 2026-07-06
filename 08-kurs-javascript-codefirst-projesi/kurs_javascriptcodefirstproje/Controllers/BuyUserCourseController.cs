using kurs_javascriptcodefirstproje.Models;
using Microsoft.AspNetCore.Mvc;

namespace kurs_javascriptcodefirstproje.Controllers
{
    public class BuyUserCourseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BuyUserCourseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var values = _context.Courses.ToList();
            return View(values);
        }

        [HttpGet]
        public IActionResult Buy(int id)
        {
            var course = _context.Courses.FirstOrDefault(x => x.Id == id);
            return View(course);
        }

        [HttpPost]
        public IActionResult Buy(int courseId, Enrollment enrollment)
        {
            if (!ModelState.IsValid)
            {
                var course = _context.Courses.FirstOrDefault(x => x.Id == courseId);
                return View(course);
            }

            var newEnrollment = new Enrollment
            {
                StudentName = enrollment.StudentName,
                Email = enrollment.Email,
                Phone = enrollment.Phone,
                CourseId = courseId,
                EnrollmentDate = DateTime.Now
            };

            _context.Enrollments.Add(newEnrollment);
            _context.SaveChanges();

            TempData["Success"] = "Başvurunuz başarıyla alınmıştır.";

            return RedirectToAction("Buy", new { id = courseId });
        }
    }
}