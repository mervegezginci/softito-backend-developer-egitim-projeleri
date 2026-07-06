using kuafor_ORMproje.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;

namespace kuafor_ORMproje.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Payment
        public async Task<IActionResult> Index()
        {
            var payments = await _context.Payments
                .Include(p => p.Appointment)
                    .ThenInclude(a => a!.Customer)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a!.Service)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
            return View(payments);
        }
        // GET: Payment/Create
        public async Task<IActionResult> Create()
        {
            // We want to list appointments that don't have a payment yet
            var unpaidAppointments = await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Service)
                .Where(a => !_context.Payments.Any(p => p.AppointmentId == a.Id))
                .Select(a => new {
                    Id = a.Id,
                    DisplayText = $"{a.Customer!.FullName} - {a.Service!.ServiceName} ({a.AppointmentDate:dd.MM.yyyy})"
                })
                .ToListAsync();
            ViewData["AppointmentId"] = new SelectList(unpaidAppointments, "Id", "DisplayText");
            return View();
        }
        // POST: Payment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AppointmentId,Amount,PaymentMethod,PaymentStatus")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                payment.PaymentDate = DateTime.Now;
                _context.Add(payment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Ödeme başarıyla kaydedildi.";
                return RedirectToAction(nameof(Index));
            }
            var unpaidAppointments = await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Service)
                .Where(a => !_context.Payments.Any(p => p.AppointmentId == a.Id) || a.Id == payment.AppointmentId)
                .Select(a => new {
                    Id = a.Id,
                    DisplayText = $"{a.Customer!.FullName} - {a.Service!.ServiceName} ({a.AppointmentDate:dd.MM.yyyy})"
                })
                .ToListAsync();
            ViewData["AppointmentId"] = new SelectList(unpaidAppointments, "Id", "DisplayText", payment.AppointmentId);
            return View(payment);
        }
        // GET: Payment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            var appointments = await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Service)
                .Select(a => new {
                    Id = a.Id,
                    DisplayText = $"{a.Customer!.FullName} - {a.Service!.ServiceName} ({a.AppointmentDate:dd.MM.yyyy})"
                })
                .ToListAsync();
            ViewData["AppointmentId"] = new SelectList(appointments, "Id", "DisplayText", payment.AppointmentId);
            return View(payment);
        }
        // POST: Payment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AppointmentId,Amount,PaymentMethod,PaymentDate,PaymentStatus")] Payment payment)
        {
            if (id != payment.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Ödeme bilgileri güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.Id))
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
            var appointments = await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Service)
                .Select(a => new {
                    Id = a.Id,
                    DisplayText = $"{a.Customer!.FullName} - {a.Service!.ServiceName} ({a.AppointmentDate:dd.MM.yyyy})"
                })
                .ToListAsync();
            ViewData["AppointmentId"] = new SelectList(appointments, "Id", "DisplayText", payment.AppointmentId);
            return View(payment);
        }
        // POST: Payment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Ödeme başarıyla silindi.";
            }
            return RedirectToAction(nameof(Index));
        }
        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.Id == id);
        }
    }
}
