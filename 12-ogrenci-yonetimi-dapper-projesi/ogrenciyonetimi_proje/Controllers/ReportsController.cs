using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ogrenciyonetimi_proje.Repositories;

namespace ogrenciyonetimi_proje.Controllers
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
            _logger.LogInformation("API: Rapor sorgulanıyor. Rapor Türü: {ReportType}", reportType);
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
                _logger.LogError(ex, "API Hata: Rapor sorgulanırken hata oluştu. Rapor Türü: {ReportType}", reportType);
                return StatusCode(500, new { message = "Rapor çekilirken hata oluştu.", details = ex.Message });
            }
        }
    }
}
