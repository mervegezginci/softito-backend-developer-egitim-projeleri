using kuafor_ORMproje.Data.Repository.IRepository;
using kuafor_ORMproje.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace kuafor_ORMproje.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Employee
        [HttpGet]
        public ActionResult<IEnumerable<Employee>> GetEmployees()
        {
            return Ok(_unitOfWork.Employee.GetAll());
        }

        // GET: api/Employee/5
        [HttpGet("{id}")]
        public ActionResult<Employee> GetEmployee(int id)
        {
            var employee = _unitOfWork.Employee.GetFirstOrDefault(u => u.Id == id);
            if (employee == null)
            {
                return NotFound(new { message = $"Çalışan (ID: {id}) bulunamadı." });
            }
            return Ok(employee);
        }

        // PUT: api/Employee/5
        [HttpPut("{id}")]
        public IActionResult PutEmployee(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest(new { message = "Kimlik bilgileri eşleşmiyor." });
            }

            _unitOfWork.Employee.Update(employee);
            _unitOfWork.Save();

            return NoContent();
        }

        // POST: api/Employee
        [HttpPost]
        public ActionResult<Employee> PostEmployee(Employee employee)
        {
            _unitOfWork.Employee.Add(employee);
            _unitOfWork.Save();
            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        }

        // DELETE: api/Employee/5
        [HttpDelete("{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            var employee = _unitOfWork.Employee.GetFirstOrDefault(u => u.Id == id);
            if (employee == null)
            {
                return NotFound(new { message = $"Çalışan (ID: {id}) bulunamadı." });
            }
            _unitOfWork.Employee.Remove(employee);
            _unitOfWork.Save();
            return Ok(new { message = "Çalışan başarıyla silindi." });
        }
    }
}
