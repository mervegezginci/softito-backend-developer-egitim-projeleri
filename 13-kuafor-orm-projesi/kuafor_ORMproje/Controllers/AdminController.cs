using kuafor_ORMproje.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace kuafor_ORMproje.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var totalCustomers = await _context.Customers.CountAsync();
            var totalEmployees = await _context.Employees.CountAsync(e => e.IsActive);
            var totalServices = await _context.Services.CountAsync();

            var today = DateTime.Today;
            var todayAppointments = await _context.Appointments
                .CountAsync(a => a.AppointmentDate.Date == today);

            var totalRevenue = await _context.Payments
                .Where(p => p.PaymentStatus)
                .SumAsync(p => p.Amount);

            var recentAppointments = await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Service)
                .Include(a => a.Employee)
                .OrderByDescending(a => a.AppointmentDate)
                .Take(5)
                .ToListAsync();

            ViewBag.TotalCustomers = totalCustomers;
            ViewBag.TotalEmployees = totalEmployees;
            ViewBag.TotalServices = totalServices;
            ViewBag.TodayAppointments = todayAppointments;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.RecentAppointments = recentAppointments;

            // Weekly Revenue & Appointments charts data
            var sevenDaysAgo = DateTime.Today.AddDays(-6);
            
            var weeklyRevenueData = await _context.Payments
                .Where(p => p.PaymentStatus && p.PaymentDate >= sevenDaysAgo)
                .ToListAsync();

            var weeklyRevenueList = Enumerable.Range(0, 7)
                .Select(offset => sevenDaysAgo.AddDays(offset))
                .Select(date => new {
                    DateLabel = date.ToString("dd MMM"),
                    Amount = weeklyRevenueData.Where(p => p.PaymentDate.Date == date).Sum(p => p.Amount)
                })
                .ToList();

            ViewBag.WeeklyRevenueLabels = weeklyRevenueList.Select(r => r.DateLabel).ToList();
            ViewBag.WeeklyRevenueValues = weeklyRevenueList.Select(r => r.Amount).ToList();

            var weeklyAppointmentsData = await _context.Appointments
                .Where(a => a.AppointmentDate >= sevenDaysAgo)
                .ToListAsync();

            var weeklyAppointmentsList = Enumerable.Range(0, 7)
                .Select(offset => sevenDaysAgo.AddDays(offset))
                .Select(date => new {
                    DateLabel = date.ToString("dd MMM"),
                    Count = weeklyAppointmentsData.Where(a => a.AppointmentDate.Date == date).Count()
                })
                .ToList();

            ViewBag.WeeklyAppointmentsLabels = weeklyAppointmentsList.Select(a => a.DateLabel).ToList();
            ViewBag.WeeklyAppointmentsValues = weeklyAppointmentsList.Select(a => a.Count).ToList();

            return View();
        }
    }
}
