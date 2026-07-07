using kuafor_ORMproje.Data.Repository.IRepository;
using kuafor_ORMproje.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace kuafor_ORMproje.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        // GET: Appointment
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var appointments = _unitOfWork.Appointment
                .GetAll(includeProperties: "Customer,Employee,Service")
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();
            return View(appointments);
        }

        // GET: Appointment/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_unitOfWork.Customer.GetAll().OrderBy(c => c.FullName).ToList(), "Id", "FullName");
            ViewData["EmployeeId"] = new SelectList(_unitOfWork.Employee.GetAll(e => e.IsActive).OrderBy(e => e.FullName).ToList(), "Id", "FullName");
            ViewData["ServiceId"] = new SelectList(_unitOfWork.Service.GetAll().OrderBy(s => s.ServiceName).ToList(), "Id", "ServiceName");
            return View();
        }

        // POST: Appointment/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,CustomerId,EmployeeId,ServiceId,AppointmentDate,AppointmentTime,Status,Note")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Appointment.Add(appointment);
                _unitOfWork.Save();
                TempData["SuccessMessage"] = "Randevu başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_unitOfWork.Customer.GetAll().OrderBy(c => c.FullName).ToList(), "Id", "FullName", appointment.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_unitOfWork.Employee.GetAll(e => e.IsActive).OrderBy(e => e.FullName).ToList(), "Id", "FullName", appointment.EmployeeId);
            ViewData["ServiceId"] = new SelectList(_unitOfWork.Service.GetAll().OrderBy(s => s.ServiceName).ToList(), "Id", "ServiceName", appointment.ServiceId);
            return View(appointment);
        }

        // GET: Appointment/Edit/5
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var appointment = _unitOfWork.Appointment.GetFirstOrDefault(u => u.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_unitOfWork.Customer.GetAll().OrderBy(c => c.FullName).ToList(), "Id", "FullName", appointment.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_unitOfWork.Employee.GetAll(e => e.IsActive).OrderBy(e => e.FullName).ToList(), "Id", "FullName", appointment.EmployeeId);
            ViewData["ServiceId"] = new SelectList(_unitOfWork.Service.GetAll().OrderBy(s => s.ServiceName).ToList(), "Id", "ServiceName", appointment.ServiceId);
            return View(appointment);
        }

        // POST: Appointment/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,CustomerId,EmployeeId,ServiceId,AppointmentDate,AppointmentTime,Status,Note")] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Appointment.Update(appointment);
                _unitOfWork.Save();
                TempData["SuccessMessage"] = "Randevu başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_unitOfWork.Customer.GetAll().OrderBy(c => c.FullName).ToList(), "Id", "FullName", appointment.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_unitOfWork.Employee.GetAll(e => e.IsActive).OrderBy(e => e.FullName).ToList(), "Id", "FullName", appointment.EmployeeId);
            ViewData["ServiceId"] = new SelectList(_unitOfWork.Service.GetAll().OrderBy(s => s.ServiceName).ToList(), "Id", "ServiceName", appointment.ServiceId);
            return View(appointment);
        }

        // POST: Appointment/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var appointment = _unitOfWork.Appointment.GetFirstOrDefault(u => u.Id == id);
            if (appointment != null)
            {
                _unitOfWork.Appointment.Remove(appointment);
                _unitOfWork.Save();
                TempData["SuccessMessage"] = "Randevu başarıyla silindi.";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Appointment/Book
        [Authorize]
        public IActionResult Book(int? serviceId)
        {
            ViewData["EmployeeId"] = new SelectList(_unitOfWork.Employee.GetAll(e => e.IsActive).OrderBy(e => e.FullName).ToList(), "Id", "FullName");
            ViewData["ServiceId"] = new SelectList(_unitOfWork.Service.GetAll().OrderBy(s => s.ServiceName).ToList(), "Id", "ServiceName", serviceId);
            return View();
        }

        // POST: Appointment/Book
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(int employeeId, int serviceId, DateTime appointmentDate, string appointmentTime, string note, string phone)
        {
            // Parse appointmentTime from string to TimeSpan
            if (!TimeSpan.TryParse(appointmentTime, out var timeSpan))
            {
                timeSpan = new TimeSpan(9, 0, 0); // default 09:00
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            // Find or create customer
            var customer = _unitOfWork.Customer.GetFirstOrDefault(c => c.Email == user.Email);
            if (customer == null)
            {
                customer = new Customer
                {
                    FullName = user.Name,
                    Phone = phone ?? "",
                    Email = user.Email!,
                    Gender = "Belirtilmemiş",
                    CreatedDate = DateTime.Now
                };
                _unitOfWork.Customer.Add(customer);
                _unitOfWork.Save();
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(phone) && customer.Phone != phone)
                {
                    customer.Phone = phone;
                    _unitOfWork.Customer.Update(customer);
                    _unitOfWork.Save();
                }
            }

            // Create appointment
            var appointment = new Appointment
            {
                CustomerId = customer.Id,
                EmployeeId = employeeId,
                ServiceId = serviceId,
                AppointmentDate = appointmentDate,
                AppointmentTime = timeSpan,
                Status = "Bekliyor",
                Note = note
            };

            _unitOfWork.Appointment.Add(appointment);
            _unitOfWork.Save();

            TempData["SuccessMessage"] = "Randevunuz başarıyla oluşturuldu! Onay sürecinden sonra sizinle iletişime geçilecektir.";
            return RedirectToAction("Index", "Home");
        }
    }
}
