using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using seyahat_projesi.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace seyahat_projesi.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingsApiController : ControllerBase
    {
        private readonly DapperRepository _dapper;

        public BookingsApiController(DapperRepository dapper)
        {
            _dapper = dapper;
        }

        // GET: api/BookingsApi
        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            var isAdmin = User.IsInRole("Admin");
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            string sql;
            var parameters = new Dictionary<string, object>();

            if (isAdmin)
            {
                sql = @"SELECT b.Id, b.BookingDate, b.GuestsCount, b.TotalPrice, b.Status, b.PaymentStatus,
                               u.Name AS UserName, u.Email AS UserEmail,
                               t.Title AS TourTitle, t.Location AS TourLocation
                        FROM Bookings b
                        INNER JOIN AspNetUsers u ON b.UserId = u.Id
                        INNER JOIN Tours t ON b.TourId = t.Id
                        ORDER BY b.Id DESC";
            }
            else
            {
                sql = @"SELECT b.Id, b.BookingDate, b.GuestsCount, b.TotalPrice, b.Status, b.PaymentStatus,
                               t.Title AS TourTitle, t.Location AS TourLocation
                        FROM Bookings b
                        INNER JOIN Tours t ON b.TourId = t.Id
                        WHERE b.UserId = @UserId
                        ORDER BY b.Id DESC";
                parameters.Add("UserId", userId ?? "");
            }

            var bookings = await _dapper.QueryAsync<dynamic>(sql, parameters);
            return Ok(bookings);
        }
    }
}
