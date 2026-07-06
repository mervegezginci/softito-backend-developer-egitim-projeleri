using kuafor_ORMproje.Data;
using kuafor_ORMproje.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Authorization;

namespace kuafor_ORMproje.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Appointment
        public async Task<IActionResult> Index()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .Include(a => a.Service)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
            return View(appointments);
        }
        // GET: Appointment/Create
        public async Task<IActionResult> Create()
        {
            ViewData["CustomerId"] = new SelectList(await _context.Customers.OrderBy(c => c.FullName).ToListAsync(), "Id", "FullName");
            ViewData["EmployeeId"] = new SelectList(await _context.Employees.Where(e => e.IsActive).OrderBy(e => e.FullName).ToListAsync(), "Id", "FullName");
            ViewData["ServiceId"] = new SelectList(await _context.Services.OrderBy(s => s.ServiceName).ToListAsync(), "Id", "ServiceName");
            return View();
        }
        // POST: Appointment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CustomerId,EmployeeId,ServiceId,AppointmentDate,AppointmentTime,Status,Note")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Randevu başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(await _context.Customers.OrderBy(c => c.FullName).ToListAsync(), "Id", "FullName", appointment.CustomerId);
            ViewData["EmployeeId"] = new SelectList(await _context.Employees.Where(e => e.IsActive).OrderBy(e => e.FullName).ToListAsync(), "Id", "FullName", appointment.EmployeeId);
            ViewData["ServiceId"] = new SelectList(await _context.Services.OrderBy(s => s.ServiceName).ToListAsync(), "Id", "ServiceName", appointment.ServiceId);
            return View(appointment);
        }
        // GET: Appointment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(await _context.Customers.OrderBy(c => c.FullName).ToListAsync(), "Id", "FullName", appointment.CustomerId);
            ViewData["EmployeeId"] = new SelectList(await _context.Employees.Where(e => e.IsActive).OrderBy(e => e.FullName).ToListAsync(), "Id", "FullName", appointment.EmployeeId);
            ViewData["ServiceId"] = new SelectList(await _context.Services.OrderBy(s => s.ServiceName).ToListAsync(), "Id", "ServiceName", appointment.ServiceId);
            return View(appointment);
        }
        // POST: Appointment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerId,EmployeeId,ServiceId,AppointmentDate,AppointmentTime,Status,Note")] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Randevu başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(await _context.Customers.OrderBy(c => c.FullName).ToListAsync(), "Id", "FullName", appointment.CustomerId);
            ViewData["EmployeeId"] = new SelectList(await _context.Employees.Where(e => e.IsActive).OrderBy(e => e.FullName).ToListAsync(), "Id", "FullName", appointment.EmployeeId);
            ViewData["ServiceId"] = new SelectList(await _context.Services.OrderBy(s => s.ServiceName).ToListAsync(), "Id", "ServiceName", appointment.ServiceId);
            return View(appointment);
        }
        // POST: Appointment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Randevu başarıyla silindi.";
            }
            return RedirectToAction(nameof(Index));
        }
        // GET: Appointment/Book
        [AllowAnonymous]
        public async Task<IActionResult> Book(int? serviceId)
        {
            ViewData["EmployeeId"] = new SelectList(await _context.Employees.Where(e => e.IsActive).OrderBy(e => e.FullName).ToListAsync(), "Id", "FullName");
            ViewData["ServiceId"] = new SelectList(await _context.Services.OrderBy(s => s.ServiceName).ToListAsync(), "Id", "ServiceName", serviceId);
            return View();
        }

        // POST: Appointment/Book
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(string fullName, string phone, string email, string gender, int employeeId, int serviceId, DateTime appointmentDate, string appointmentTime, string note)
        {
            // Parse appointmentTime from string to TimeSpan
            if (!TimeSpan.TryParse(appointmentTime, out var timeSpan))
            {
                timeSpan = new TimeSpan(9, 0, 0); // default 09:00
            }

            // Find or create customer
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email || c.Phone == phone);
            if (customer == null)
            {
                customer = new Customer
                {
                    FullName = fullName,
                    Phone = phone,
                    Email = email,
                    Gender = gender,
                    CreatedDate = DateTime.Now
                };
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
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

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Randevunuz başarıyla oluşturuldu! Onay sürecinden sonra sizinle iletişime geçilecektir.";
            return RedirectToAction("Index", "Home");
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}
