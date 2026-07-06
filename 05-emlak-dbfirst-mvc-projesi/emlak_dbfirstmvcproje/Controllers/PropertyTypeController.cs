using Microsoft.AspNetCore.Mvc;
using emlak_dbfirstmvcproje.Models;

namespace emlak_dbfirstmvcproje.Controllers
{
    public class PropertyTypeController : Controller
    {
        private readonly RealEstateDbContext _context;

        public PropertyTypeController(RealEstateDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search)
        {
            var values = _context.PropertyTypes.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                values = values.Where(x =>
                    x.TypeName.Contains(search));
            }

            return View(values.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(PropertyType propertyType)
        {
            _context.PropertyTypes.Add(propertyType);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var value = _context.PropertyTypes.Find(id);

            if (value == null)
                return NotFound();

            return View(value);
        }

        [HttpPost]
        public IActionResult Edit(PropertyType propertyType)
        {
            _context.PropertyTypes.Update(propertyType);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var value = _context.PropertyTypes.Find(id);

            if (value == null)
                return NotFound();

            _context.PropertyTypes.Remove(value);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}