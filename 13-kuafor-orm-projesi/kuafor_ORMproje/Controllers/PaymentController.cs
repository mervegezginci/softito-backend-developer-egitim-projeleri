using kuafor_ORMproje.Data.Repository.IRepository;
using kuafor_ORMproje.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;

namespace kuafor_ORMproje.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Payment
        public IActionResult Index()
        {
            var payments = _unitOfWork.Payment
                .GetAll(includeProperties: "Appointment,Appointment.Customer,Appointment.Service")
                .OrderByDescending(p => p.PaymentDate)
                .ToList();
            return View(payments);
        }

        // GET: Payment/Create
        public IActionResult Create()
        {
            var paidAppointmentIds = _unitOfWork.Payment.GetAll().Select(p => p.AppointmentId).ToList();
            var unpaidAppointments = _unitOfWork.Appointment
                .GetAll(a => !paidAppointmentIds.Contains(a.Id) && a.Status != "İptal Edildi", includeProperties: "Customer,Service")
                .Select(a => new {
                    Id = a.Id,
                    DisplayText = $"{a.Customer!.FullName} - {a.Service!.ServiceName} ({a.AppointmentDate:dd.MM.yyyy})",
                    Price = a.Service!.Price
                })
                .ToList();
            ViewBag.UnpaidAppointments = unpaidAppointments;
            return View();
        }

        // POST: Payment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,AppointmentId,Amount,PaymentMethod,PaymentStatus")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                payment.PaymentDate = DateTime.Now;
                _unitOfWork.Payment.Add(payment);
                _unitOfWork.Save();
                TempData["SuccessMessage"] = "Ödeme başarıyla kaydedildi.";
                return RedirectToAction(nameof(Index));
            }
            var paidAppointmentIds = _unitOfWork.Payment.GetAll().Select(p => p.AppointmentId).ToList();
            var unpaidAppointments = _unitOfWork.Appointment
                .GetAll(a => (!paidAppointmentIds.Contains(a.Id) || a.Id == payment.AppointmentId) && a.Status != "İptal Edildi", includeProperties: "Customer,Service")
                .Select(a => new {
                    Id = a.Id,
                    DisplayText = $"{a.Customer!.FullName} - {a.Service!.ServiceName} ({a.AppointmentDate:dd.MM.yyyy})",
                    Price = a.Service!.Price
                })
                .ToList();
            ViewBag.UnpaidAppointments = unpaidAppointments;
            return View(payment);
        }

        // GET: Payment/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var payment = _unitOfWork.Payment.GetFirstOrDefault(u => u.Id == id);
            if (payment == null)
            {
                return NotFound();
            }
            var appointments = _unitOfWork.Appointment
                .GetAll(includeProperties: "Customer,Service")
                .Select(a => new {
                    Id = a.Id,
                    DisplayText = $"{a.Customer!.FullName} - {a.Service!.ServiceName} ({a.AppointmentDate:dd.MM.yyyy})"
                })
                .ToList();
            ViewData["AppointmentId"] = new SelectList(appointments, "Id", "DisplayText", payment.AppointmentId);
            return View(payment);
        }

        // POST: Payment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,AppointmentId,Amount,PaymentMethod,PaymentDate,PaymentStatus")] Payment payment)
        {
            if (id != payment.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Payment.Update(payment);
                _unitOfWork.Save();
                TempData["SuccessMessage"] = "Ödeme bilgileri güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            var appointments = _unitOfWork.Appointment
                .GetAll(includeProperties: "Customer,Service")
                .Select(a => new {
                    Id = a.Id,
                    DisplayText = $"{a.Customer!.FullName} - {a.Service!.ServiceName} ({a.AppointmentDate:dd.MM.yyyy})"
                })
                .ToList();
            ViewData["AppointmentId"] = new SelectList(appointments, "Id", "DisplayText", payment.AppointmentId);
            return View(payment);
        }

        // POST: Payment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var payment = _unitOfWork.Payment.GetFirstOrDefault(u => u.Id == id);
            if (payment != null)
            {
                _unitOfWork.Payment.Remove(payment);
                _unitOfWork.Save();
                TempData["SuccessMessage"] = "Ödeme başarıyla silindi.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
