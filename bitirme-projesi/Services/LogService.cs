using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using seyahat_projesi.Data;
using seyahat_projesi.Model;

namespace seyahat_projesi.Services
{
    public class LogService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogActionAsync(string? userId, string action, string level = "info", string? details = null)
        {
            string ipAddress = "127.0.0.1";
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
                if (ipAddress == "::1" || ipAddress == "::ffff:127.0.0.1")
                {
                    ipAddress = "127.0.0.1";
                }
            }

            var logEntry = new SystemLog
            {
                UserId = userId,
                Action = action,
                Level = level,
                IpAddress = ipAddress,
                Details = details,
                CreatedAt = DateTime.Now
            };

            _context.SystemLogs.Add(logEntry);
            await _context.SaveChangesAsync();
        }
    }
}
