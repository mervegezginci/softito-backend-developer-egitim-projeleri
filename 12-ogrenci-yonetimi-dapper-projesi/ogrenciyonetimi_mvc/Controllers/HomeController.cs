using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ogrenciyonetimi_mvc.Models;
using ogrenciyonetimi_mvc.Services;

namespace ogrenciyonetimi_mvc.Controllers;

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
        _logger.LogInformation("Landing page görüntülendi.");
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Dashboard()
    {
        _logger.LogInformation("Dashboard sayfası görüntülendi.");

        var students = await _api.GetStudentsAsync();
        var departments = await _api.GetDepartmentsAsync();
        var grades = await _api.GetGradesAsync();

        ViewBag.TotalStudents = students.Count();
        ViewBag.TotalDepartments = departments.Count();
        ViewBag.TotalGrades = grades.Count();
        ViewBag.AverageScore = grades.Any() ? Math.Round(grades.Average(g => g.Score), 2) : 0;
        ViewBag.ActiveStudents = students.Count(s => s.IsActive);
        ViewBag.InactiveStudents = students.Count(s => !s.IsActive);

        // Son eklenen 5 öğrenci
        ViewBag.RecentStudents = students.Take(5);

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
