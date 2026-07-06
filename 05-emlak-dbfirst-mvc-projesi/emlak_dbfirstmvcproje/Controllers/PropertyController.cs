using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using emlak_dbfirstmvcproje.Models;

public class PropertyController : Controller
{
    private readonly RealEstateDbContext _context;

    public PropertyController(RealEstateDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(string search)
    {
        var values = _context.Properties
            .Include(x => x.City)
            .Include(x => x.PropertyType)
            .Include(x => x.Realtor)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            values = values.Where(x =>
                x.Title.Contains(search));
        }

        return View(values.ToList());
    }

    // CREATE GET
    public IActionResult Create()
    {
        ViewBag.Cities = _context.Cities.ToList();
        ViewBag.Types = _context.PropertyTypes.ToList();
        ViewBag.Realtors = _context.Realtors.ToList();
        return View();
    }

    // CREATE POST
    [HttpPost]
    public IActionResult Create(Property p)
    {
        _context.Properties.Add(p);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    // DELETE
    public IActionResult Delete(int id)
    {
        var item = _context.Properties.Find(id);
        _context.Properties.Remove(item);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int id)
    {
        var item = _context.Properties.Find(id);

        ViewBag.Cities = _context.Cities.ToList();
        ViewBag.Types = _context.PropertyTypes.ToList();
        ViewBag.Realtors = _context.Realtors.ToList();

        return View(item);
    }

    [HttpPost]
    public IActionResult Edit(Property p)
    {
        _context.Properties.Update(p);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    // PUBLIC SEARCH
    public IActionResult Search(string purpose, int? cityId, string type)
    {
        var query = _context.Properties
            .Include(x => x.City)
            .Include(x => x.PropertyType)
            .Include(x => x.Realtor)
            .AsQueryable();

        if (!string.IsNullOrEmpty(purpose))
        {
            query = query.Where(x => x.PropertyType.TypeName == purpose);
        }

        if (cityId.HasValue)
        {
            query = query.Where(x => x.CityId == cityId.Value);
        }

        if (!string.IsNullOrEmpty(type))
        {
            query = query.Where(x => x.Title.Contains(type) || (x.Description != null && x.Description.Contains(type)));
        }

        return View("SearchResults", query.ToList());
    }

    // PUBLIC DETAILS
    public IActionResult Details(int id)
    {
        var property = _context.Properties
            .Include(x => x.City)
            .Include(x => x.PropertyType)
            .Include(x => x.Realtor)
            .FirstOrDefault(x => x.Id == id);

        if (property == null)
        {
            return NotFound();
        }

        return View(property);
    }
}