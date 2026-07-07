using kuafor_ORMproje.Data.Repository.IRepository;
using kuafor_ORMproje.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace kuafor_ORMproje.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServiceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Service
        [HttpGet]
        public ActionResult<IEnumerable<Service>> GetServices()
        {
            return Ok(_unitOfWork.Service.GetAll());
        }

        // GET: api/Service/5
        [HttpGet("{id}")]
        public ActionResult<Service> GetService(int id)
        {
            var service = _unitOfWork.Service.GetFirstOrDefault(u => u.Id == id);
            if (service == null)
            {
                return NotFound(new { message = $"Hizmet (ID: {id}) bulunamadı." });
            }
            return Ok(service);
        }

        // PUT: api/Service/5
        [HttpPut("{id}")]
        public IActionResult PutService(int id, Service service)
        {
            if (id != service.Id)
            {
                return BadRequest(new { message = "Kimlik bilgileri eşleşmiyor." });
            }

            _unitOfWork.Service.Update(service);
            _unitOfWork.Save();

            return NoContent();
        }

        // POST: api/Service
        [HttpPost]
        public ActionResult<Service> PostService(Service service)
        {
            _unitOfWork.Service.Add(service);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(GetService), new { id = service.Id }, service);
        }

        // DELETE: api/Service/5
        [HttpDelete("{id}")]
        public IActionResult DeleteService(int id)
        {
            var service = _unitOfWork.Service.GetFirstOrDefault(u => u.Id == id);
            if (service == null)
            {
                return NotFound(new { message = $"Hizmet (ID: {id}) bulunamadı." });
            }
            _unitOfWork.Service.Remove(service);
            _unitOfWork.Save();
            return Ok(new { message = "Hizmet başarıyla silindi." });
        }
    }
}
