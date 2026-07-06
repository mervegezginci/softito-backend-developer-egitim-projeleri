using kuafor_ORMproje.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace kuafor_ORMproje.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/AppointmentsApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
        {
            return await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .Include(a => a.Service)
                .Include(a => a.Payment)
                .ToListAsync();
        }
        // GET: api/AppointmentsApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointment(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .Include(a => a.Service)
                .Include(a => a.Payment)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (appointment == null)
            {
                return NotFound(new { message = $"Randevu (ID: {id}) bulunamadı." });
            }
            return appointment;
        }
        // PUT: api/AppointmentsApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointment(int id, Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return BadRequest(new { message = "Kimlik bilgileri eşleşmiyor." });
            }
            _context.Entry(appointment).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(id))
                {
                    return NotFound(new { message = $"Randevu (ID: {id}) bulunamadı." });
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }
        // POST: api/AppointmentsApi
        [HttpPost]
        public async Task<ActionResult<Appointment>> PostAppointment(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            // Load related data
            await _context.Entry(appointment).Reference(a => a.Customer).LoadAsync();
            await _context.Entry(appointment).Reference(a => a.Employee).LoadAsync();
            await _context.Entry(appointment).Reference(a => a.Service).LoadAsync();
            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
        }
        // DELETE: api/AppointmentsApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound(new { message = $"Randevu (ID: {id}) bulunamadı." });
            }
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Randevu başarıyla silindi." });
        }
        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}
