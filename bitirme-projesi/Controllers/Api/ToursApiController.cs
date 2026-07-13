using Microsoft.AspNetCore.Mvc;
using seyahat_projesi.Data;
using seyahat_projesi.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace seyahat_projesi.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToursApiController : ControllerBase
    {
        private readonly DapperRepository _dapper;

        public ToursApiController(DapperRepository dapper)
        {
            _dapper = dapper;
        }

        // GET: api/ToursApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tour>>> GetTours(string? search, int? categoryId)
        {
            var sql = @"SELECT t.Id, t.Title, t.Description, t.Location, t.StartDate, t.EndDate, t.DurationDays, t.Price, t.Capacity, t.ImageUrl, t.IsActive, t.CategoryId, t.GuideId,
                               c.Id, c.Name, c.Description,
                               g.Id, g.FullName, g.Mail, g.Phone, g.Bio, g.GuideImageUrl
                        FROM Tours t
                        INNER JOIN Categories c ON t.CategoryId = c.Id
                        INNER JOIN Guides g ON t.GuideId = g.Id
                        WHERE t.IsActive = 1";

            var parameters = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(search))
            {
                sql += " AND (t.Title LIKE @Search OR t.Location LIKE @Search)";
                parameters.Add("Search", $"%{search}%");
            }

            if (categoryId.HasValue)
            {
                sql += " AND t.CategoryId = @CategoryId";
                parameters.Add("CategoryId", categoryId.Value);
            }

            sql += " ORDER BY t.Id DESC";

            var tours = await _dapper.QueryToursAsync(sql, parameters);
            return Ok(tours);
        }

        // GET: api/ToursApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tour>> GetTour(int id)
        {
            var sql = @"SELECT t.Id, t.Title, t.Description, t.Location, t.StartDate, t.EndDate, t.DurationDays, t.Price, t.Capacity, t.ImageUrl, t.IsActive, t.CategoryId, t.GuideId,
                               c.Id, c.Name, c.Description,
                               g.Id, g.FullName, g.Mail, g.Phone, g.Bio, g.GuideImageUrl
                        FROM Tours t
                        INNER JOIN Categories c ON t.CategoryId = c.Id
                        INNER JOIN Guides g ON t.GuideId = g.Id
                        WHERE t.Id = @Id AND t.IsActive = 1";

            var tour = (await _dapper.QueryToursAsync(sql, new { Id = id })).FirstOrDefault();

            if (tour == null)
            {
                return NotFound();
            }

            return Ok(tour);
        }
    }
}
