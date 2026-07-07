using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ogrenciyonetimi_proje.Repositories;

namespace ogrenciyonetimi_proje.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentRepository _repo;

        public DepartmentsController(IDepartmentRepository repo)
        {
            _repo = repo;
        }

        /// <summary>Tüm bölümleri listele</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var departments = await _repo.GetAllAsync();
            return Ok(departments);
        }
    }
}
