using kuafor_ORMproje.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace kuafor_ORMproje.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/PaymentsApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
        {
            return await _context.Payments
                .Include(p => p.Appointment)
                    .ThenInclude(a => a!.Customer)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a!.Service)
                .ToListAsync();
        }
        // GET: api/PaymentsApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> GetPayment(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.Appointment)
                    .ThenInclude(a => a!.Customer)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a!.Service)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (payment == null)
            {
                return NotFound(new { message = $"Ödeme (ID: {id}) bulunamadı." });
            }
            return payment;
        }
        // PUT: api/PaymentsApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPayment(int id, Payment payment)
        {
            if (id != payment.Id)
            {
                return BadRequest(new { message = "Kimlik bilgileri eşleşmiyor." });
            }
            _context.Entry(payment).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentExists(id))
                {
                    return NotFound(new { message = $"Ödeme (ID: {id}) bulunamadı." });
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }
        // POST: api/PaymentsApi
        [HttpPost]
        public async Task<ActionResult<Payment>> PostPayment(Payment payment)
        {
            payment.PaymentDate = DateTime.Now;
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            // Load related data
            await _context.Entry(payment).Reference(p => p.Appointment).LoadAsync();
            if (payment.Appointment != null)
            {
                await _context.Entry(payment.Appointment).Reference(a => a.Customer).LoadAsync();
                await _context.Entry(payment.Appointment).Reference(a => a.Service).LoadAsync();
            }
            return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, payment);
        }
        // DELETE: api/PaymentsApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound(new { message = $"Ödeme (ID: {id}) bulunamadı." });
            }
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Ödeme başarıyla silindi." });
        }
        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.Id == id);
        }
    }
}
