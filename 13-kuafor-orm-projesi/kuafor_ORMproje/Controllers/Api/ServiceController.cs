using kuafor_ORMproje.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace kuafor_ORMproje.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ServiceController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/ServicesApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        {
            return await _context.Services.ToListAsync();
        }
        // GET: api/ServicesApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Service>> GetService(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound(new { message = $"Hizmet (ID: {id}) bulunamadı." });
            }
            return service;
        }
        // PUT: api/ServicesApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutService(int id, Service service)
        {
            if (id != service.Id)
            {
                return BadRequest(new { message = "Kimlik bilgileri eşleşmiyor." });
            }
            _context.Entry(service).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(id))
                {
                    return NotFound(new { message = $"Hizmet (ID: {id}) bulunamadı." });
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }
        // POST: api/ServicesApi
        [HttpPost]
        public async Task<ActionResult<Service>> PostService(Service service)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetService), new { id = service.Id }, service);
        }
        // DELETE: api/ServicesApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound(new { message = $"Hizmet (ID: {id}) bulunamadı." });
            }
            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Hizmet başarıyla silindi." });
        }
        private bool ServiceExists(int id)
        {
            return _context.Services.Any(e => e.Id == id);
        }
    }
}
