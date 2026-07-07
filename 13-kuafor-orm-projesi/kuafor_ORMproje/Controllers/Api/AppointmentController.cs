using kuafor_ORMproje.Data.Repository.IRepository;
using kuafor_ORMproje.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace kuafor_ORMproje.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Appointment
        [HttpGet]
        public ActionResult<IEnumerable<Appointment>> GetAppointments()
        {
            return Ok(_unitOfWork.Appointment.GetAll(includeProperties: "Customer,Employee,Service,Payment"));
        }

        // GET: api/Appointment/5
        [HttpGet("{id}")]
        public ActionResult<Appointment> GetAppointment(int id)
        {
            var appointment = _unitOfWork.Appointment.GetFirstOrDefault(
                a => a.Id == id, 
                includeProperties: "Customer,Employee,Service,Payment"
            );
            if (appointment == null)
            {
                return NotFound(new { message = $"Randevu (ID: {id}) bulunamadı." });
            }
            return Ok(appointment);
        }

        // PUT: api/Appointment/5
        [HttpPut("{id}")]
        public IActionResult PutAppointment(int id, Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return BadRequest(new { message = "Kimlik bilgileri eşleşmiyor." });
            }

            _unitOfWork.Appointment.Update(appointment);
            _unitOfWork.Save();

            return NoContent();
        }

        // POST: api/Appointment
        [HttpPost]
        public ActionResult<Appointment> PostAppointment(Appointment appointment)
        {
            _unitOfWork.Appointment.Add(appointment);
            _unitOfWork.Save();

            // Load related data
            var savedAppointment = _unitOfWork.Appointment.GetFirstOrDefault(
                a => a.Id == appointment.Id,
                includeProperties: "Customer,Employee,Service"
            );

            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, savedAppointment);
        }

        // DELETE: api/Appointment/5
        [HttpDelete("{id}")]
        public IActionResult DeleteAppointment(int id)
        {
            var appointment = _unitOfWork.Appointment.GetFirstOrDefault(u => u.Id == id);
            if (appointment == null)
            {
                return NotFound(new { message = $"Randevu (ID: {id}) bulunamadı." });
            }
            _unitOfWork.Appointment.Remove(appointment);
            _unitOfWork.Save();
            return Ok(new { message = "Randevu başarıyla silindi." });
        }
    }
}
