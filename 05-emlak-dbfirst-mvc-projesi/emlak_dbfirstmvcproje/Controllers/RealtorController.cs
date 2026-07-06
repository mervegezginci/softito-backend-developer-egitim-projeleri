using Microsoft.AspNetCore.Mvc;
using emlak_dbfirstmvcproje.Models;

namespace emlak_dbfirstmvcproje.Controllers
{
    public class RealtorController : Controller
    {
        private readonly RealEstateDbContext _context;

        public RealtorController(RealEstateDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search)
        {
            var values = _context.Realtors.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                values = values.Where(x =>
                    x.NameSurname.Contains(search));
            }

            return View(values.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Realtor realtor)
        {
            _context.Realtors.Add(realtor);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var value = _context.Realtors.Find(id);

            if (value == null)
                return NotFound();

            return View(value);
        }

        [HttpPost]
        public IActionResult Edit(Realtor realtor)
        {
            _context.Realtors.Update(realtor);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var value = _context.Realtors.Find(id);

            if (value == null)
                return NotFound();

            _context.Realtors.Remove(value);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}