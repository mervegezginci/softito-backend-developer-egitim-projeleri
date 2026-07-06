
using kuafor_ORMproje.Data.Repository;
using kuafor_ORMproje.Model;
using kuafor_ORMproje.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace kuafor_ORMproje.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var services = await _context.Services.ToListAsync();
            var employees = await _context.Employees.Where(e => e.IsActive).ToListAsync();
            var customers = await _context.Customers.ToListAsync();

            var viewModel = new HomeViewModel
            {
                Services = services,
                Employees = employees,
                Customers = customers
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
