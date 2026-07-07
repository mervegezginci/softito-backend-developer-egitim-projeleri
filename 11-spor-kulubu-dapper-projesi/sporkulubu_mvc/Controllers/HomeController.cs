using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using sporkulubu_mvc.Models;
using sporkulubu_mvc.Services;

namespace sporkulubu_mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiService _api;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApiService api, ILogger<HomeController> logger)
        {
            _api = api;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogInformation("Landing page görüntüleniyor.");
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            _logger.LogInformation("Dashboard sayfası görüntüleniyor.");

            var members = await _api.GetMembersAsync();
            var branches = await _api.GetBranchesAsync();
            var trainings = await _api.GetTrainingsAsync();

            ViewBag.TotalStudents = members.Count();
            ViewBag.TotalDepartments = branches.Count();
            ViewBag.TotalGrades = trainings.Count();
            ViewBag.AverageScore = trainings.Any() ? Math.Round(trainings.Average(t => t.Fee), 2) : 0.00m;
            ViewBag.ActiveStudents = members.Count(m => m.IsActive);
            ViewBag.InactiveStudents = members.Count(m => !m.IsActive);
            ViewBag.RecentStudents = members.Take(5);

            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetReportData(string reportType)
        {
            _logger.LogInformation("Dashboard AJAX Rapor İstendi. Tür: {Type}", reportType);
            var json = await _api.GetReportJsonAsync(reportType);
            return Content(json, "application/json");
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
