using Microsoft.AspNetCore.Mvc;
using kurs_javascriptcodefirstproje.Models;

namespace kurs_javascriptcodefirstproje.Controllers
{
    public class EnrollmentController : Controller
    {
        private readonly ApplicationDbContext context;

        public EnrollmentController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public JsonResult CourseList()
        {
            var data = context.Courses
                .Select(x => new
                {
                    x.Id,
                    x.CourseName
                })
                .ToList();

            return new JsonResult(data);
        }

        public JsonResult EnrollmentList(string search = "")
        {
            var data = context.Enrollments.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x =>
                    x.StudentName.Contains(search) ||
                    x.Email.Contains(search) ||
                    x.Phone.Contains(search) ||
                    x.Course.CourseName.Contains(search));
            }

            return Json(data.Select(x => new
            {
                x.Id,
                x.StudentName,
                x.Email,
                x.Phone,
                x.EnrollmentDate,
                CourseName = x.Course.CourseName
            }).ToList());
        }

        // ADD
        [HttpPost]
        public JsonResult AddEnrollment(Enrollment enrollment)
        {
            if (enrollment.CourseId == 0)
            {
                return Json("Invalid CourseId");
            }

            var e = new Enrollment()
            {
                StudentName = enrollment.StudentName,
                Email = enrollment.Email,
                Phone = enrollment.Phone,
                CourseId = enrollment.CourseId,
                EnrollmentDate = DateTime.Now
            };

            context.Enrollments.Add(e);
            context.SaveChanges();

            return new JsonResult("Enrollment saved");
        }

        // EDIT
        public JsonResult Edit(int id)
        {
            var data = context.Enrollments
                .Where(x => x.Id == id)
                .SingleOrDefault();

            return new JsonResult(data);
        }

        // UPDATE
        [HttpPost]
        public JsonResult Update(Enrollment enrollment)
        {
            var data = context.Enrollments.FirstOrDefault(x => x.Id == enrollment.Id);

            if (data == null)
                return Json("Not found");

            data.StudentName = enrollment.StudentName;
            data.Email = enrollment.Email;
            data.Phone = enrollment.Phone;
            data.CourseId = enrollment.CourseId;

            context.SaveChanges();

            return Json("Updated");
        }

        // DELETE
        public JsonResult Delete(int id)
        {
            var data = context.Enrollments
                .Where(x => x.Id == id)
                .SingleOrDefault();

            context.Enrollments.Remove(data);
            context.SaveChanges();

            return new JsonResult("Enrollment deleted");
        }
    }
}