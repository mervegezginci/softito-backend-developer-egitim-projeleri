using Microsoft.AspNetCore.Mvc;
using emlak_dbfirstmvcproje.Models;

public class HomeController : Controller
{
    private readonly RealEstateDbContext _context;

    public HomeController(RealEstateDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var properties = _context.Properties.ToList();
        var cities = _context.Cities.ToList();
        var realtors = _context.Realtors.ToList();

        ViewBag.Cities = cities;
        ViewBag.Realtors = realtors;

        return View(properties);
    }
}