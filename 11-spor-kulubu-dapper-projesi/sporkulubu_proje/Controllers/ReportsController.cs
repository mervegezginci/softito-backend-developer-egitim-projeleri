using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using sporkulubu_proje.Repositories;

namespace sporkulubu_proje.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IReportRepository _repo;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(IReportRepository repo, ILogger<ReportsController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet("{reportType}")]
        public async Task<IActionResult> GetReport(string reportType)
        {
            _logger.LogInformation("API: Spor kulübü raporu sorgulanıyor. Tür: {Type}", reportType);
            try
            {
                var result = await _repo.GetReportAsync(reportType);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API Hata: Rapor sorgulanırken hata oluştu. Tür: {Type}", reportType);
                return StatusCode(500, new { message = "Rapor alınırken hata oluştu.", details = ex.Message });
            }
        }
    }
}
