using kuafor_ORMproje.Data;
using kuafor_ORMproje.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
namespace kuafor_ORMproje.Controllers
{
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
        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}
