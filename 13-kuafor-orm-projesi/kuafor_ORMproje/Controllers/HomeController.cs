using kuafor_ORMproje.Data.Repository.IRepository;
using kuafor_ORMproje.Model;
using kuafor_ORMproje.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;

namespace kuafor_ORMproje.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var services = _unitOfWork.Service.GetAll().ToList();
            var employees = _unitOfWork.Employee.GetAll(e => e.IsActive).ToList();
            var customers = _unitOfWork.Customer.GetAll().ToList();

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
