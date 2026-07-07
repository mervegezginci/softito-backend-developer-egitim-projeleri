using kuafor_ORMproje.Data.Repository.IRepository;
using kuafor_ORMproje.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace kuafor_ORMproje.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var totalCustomers = _unitOfWork.Customer.GetAll().Count();
            var totalEmployees = _unitOfWork.Employee.GetAll(e => e.IsActive).Count();
            var totalServices = _unitOfWork.Service.GetAll().Count();

            var today = DateTime.Today;
            var todayAppointments = _unitOfWork.Appointment
                .GetAll(a => a.AppointmentDate.Date == today).Count();

            var totalRevenue = _unitOfWork.Payment
                .GetAll(p => p.PaymentStatus)
                .Sum(p => p.Amount);

            var recentAppointments = _unitOfWork.Appointment
                .GetAll(includeProperties: "Customer,Service,Employee")
                .OrderByDescending(a => a.AppointmentDate)
                .Take(5)
                .ToList();

            ViewBag.TotalCustomers = totalCustomers;
            ViewBag.TotalEmployees = totalEmployees;
            ViewBag.TotalServices = totalServices;
            ViewBag.TodayAppointments = todayAppointments;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.RecentAppointments = recentAppointments;

            // Weekly Revenue & Appointments charts data
            var sevenDaysAgo = DateTime.Today.AddDays(-6);
            
            var weeklyRevenueData = _unitOfWork.Payment
                .GetAll(p => p.PaymentStatus && p.PaymentDate >= sevenDaysAgo)
                .ToList();

            var weeklyRevenueList = Enumerable.Range(0, 7)
                .Select(offset => sevenDaysAgo.AddDays(offset))
                .Select(date => new {
                    DateLabel = date.ToString("dd MMM"),
                    Amount = weeklyRevenueData.Where(p => p.PaymentDate.Date == date).Sum(p => p.Amount)
                })
                .ToList();

            ViewBag.WeeklyRevenueLabels = weeklyRevenueList.Select(r => r.DateLabel).ToList();
            ViewBag.WeeklyRevenueValues = weeklyRevenueList.Select(r => r.Amount).ToList();

            var weeklyAppointmentsData = _unitOfWork.Appointment
                .GetAll(a => a.AppointmentDate >= sevenDaysAgo)
                .ToList();

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
