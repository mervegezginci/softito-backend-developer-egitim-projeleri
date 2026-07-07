using System.Security.Cryptography;
using System.Text;
using Dapper;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ogrenciyonetimi_proje.Data;
using ogrenciyonetimi_proje.Models;

namespace ogrenciyonetimi_proje.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message)> RegisterAsync(RegisterDto dto);
        Task<(bool Success, string Token, User? User)> LoginAsync(LoginDto dto);
    }

    public class AuthService : IAuthService
    {
        private readonly DbConnectionFactory _db;
        private readonly IConfiguration _config;

        public AuthService(DbConnectionFactory db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterDto dto)
        {
            var hash = HashPassword(dto.Password);
            using var conn = _db.CreateConnection();
            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "sp_Register",
                new { dto.Username, dto.Email, PasswordHash = hash, Role = "User" },
                commandType: System.Data.CommandType.StoredProcedure);

            int resultCode = (int)(result?.Result ?? -1);
            string message = (string)(result?.Message ?? "Hata oluştu.");

            return (resultCode > 0, message);
        }

        public async Task<(bool Success, string Token, User? User)> LoginAsync(LoginDto dto)
        {
            var hash = HashPassword(dto.Password);
            using var conn = _db.CreateConnection();
            var user = await conn.QueryFirstOrDefaultAsync<User>(
                "sp_Login",
                new { dto.Email, PasswordHash = hash },
                commandType: System.Data.CommandType.StoredProcedure);

            if (user == null)
                return (false, string.Empty, null);

            var token = GenerateJwtToken(user);
            return (true, token, user);
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
