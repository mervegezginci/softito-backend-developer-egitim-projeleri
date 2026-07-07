using kuafor_ORMproje.Data.Repository.IRepository;
using kuafor_ORMproje.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace kuafor_ORMproje.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Customer
        [HttpGet]
        public ActionResult<IEnumerable<Customer>> GetCustomers()
        {
            return Ok(_unitOfWork.Customer.GetAll());
        }

        // GET: api/Customer/5
        [HttpGet("{id}")]
        public ActionResult<Customer> GetCustomer(int id)
        {
            var customer = _unitOfWork.Customer.GetFirstOrDefault(u => u.Id == id);

            if (customer == null)
            {
                return NotFound(new { message = $"Müşteri (ID: {id}) bulunamadı." });
            }

            return Ok(customer);
        }

        // PUT: api/Customer/5
        [HttpPut("{id}")]
        public IActionResult PutCustomer(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                return BadRequest(new { message = "Kimlik bilgileri eşleşmiyor." });
            }

            _unitOfWork.Customer.Update(customer);
            _unitOfWork.Save();

            return NoContent();
        }

        // POST: api/Customer
        [HttpPost]
        public ActionResult<Customer> PostCustomer(Customer customer)
        {
            customer.CreatedDate = DateTime.Now;
            _unitOfWork.Customer.Add(customer);
            _unitOfWork.Save();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }

        // DELETE: api/Customer/5
        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            var customer = _unitOfWork.Customer.GetFirstOrDefault(u => u.Id == id);
            if (customer == null)
            {
                return NotFound(new { message = $"Müşteri (ID: {id}) bulunamadı." });
            }

            _unitOfWork.Customer.Remove(customer);
            _unitOfWork.Save();

            return Ok(new { message = "Müşteri başarıyla silindi." });
        }
    }
}
