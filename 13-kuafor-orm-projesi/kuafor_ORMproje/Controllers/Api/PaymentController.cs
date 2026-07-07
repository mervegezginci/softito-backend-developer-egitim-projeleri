using kuafor_ORMproje.Data.Repository.IRepository;
using kuafor_ORMproje.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace kuafor_ORMproje.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Payment
        [HttpGet]
        public ActionResult<IEnumerable<Payment>> GetPayments()
        {
            return Ok(_unitOfWork.Payment.GetAll(includeProperties: "Appointment,Appointment.Customer,Appointment.Service"));
        }

        // GET: api/Payment/5
        [HttpGet("{id}")]
        public ActionResult<Payment> GetPayment(int id)
        {
            var payment = _unitOfWork.Payment.GetFirstOrDefault(
                p => p.Id == id,
                includeProperties: "Appointment,Appointment.Customer,Appointment.Service"
            );
            if (payment == null)
            {
                return NotFound(new { message = $"Ödeme (ID: {id}) bulunamadı." });
            }
            return Ok(payment);
        }

        // PUT: api/Payment/5
        [HttpPut("{id}")]
        public IActionResult PutPayment(int id, Payment payment)
        {
            if (id != payment.Id)
            {
                return BadRequest(new { message = "Kimlik bilgileri eşleşmiyor." });
            }

            _unitOfWork.Payment.Update(payment);
            _unitOfWork.Save();

            return NoContent();
        }

        // POST: api/Payment
        [HttpPost]
        public ActionResult<Payment> PostPayment(Payment payment)
        {
            payment.PaymentDate = DateTime.Now;
            _unitOfWork.Payment.Add(payment);
            _unitOfWork.Save();

            // Load related data
            var savedPayment = _unitOfWork.Payment.GetFirstOrDefault(
                p => p.Id == payment.Id,
                includeProperties: "Appointment,Appointment.Customer,Appointment.Service"
            );

            return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, savedPayment);
        }

        // DELETE: api/Payment/5
        [HttpDelete("{id}")]
        public IActionResult DeletePayment(int id)
        {
            var payment = _unitOfWork.Payment.GetFirstOrDefault(u => u.Id == id);
            if (payment == null)
            {
                return NotFound(new { message = $"Ödeme (ID: {id}) bulunamadı." });
            }
            _unitOfWork.Payment.Remove(payment);
            _unitOfWork.Save();
            return Ok(new { message = "Ödeme başarıyla silindi." });
        }
    }
}
