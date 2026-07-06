using Microsoft.AspNetCore.Mvc;
using emlak_dbfirstmvcproje.Models;

namespace emlak_dbfirstmvcproje.Controllers
{
    public class CityController : Controller
    {
        private readonly RealEstateDbContext _context;

        public CityController(RealEstateDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search)
        {
            var values = _context.Cities.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                values = values.Where(x =>
                    x.Name.Contains(search));
            }

            return View(values.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(City city)
        {
            _context.Cities.Add(city);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var value = _context.Cities.Find(id);

            if (value == null)
                return NotFound();

            return View(value);
        }

        [HttpPost]
        public IActionResult Edit(City city)
        {
            _context.Cities.Update(city);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var value = _context.Cities.Find(id);

            if (value == null)
                return NotFound();

            _context.Cities.Remove(value);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}