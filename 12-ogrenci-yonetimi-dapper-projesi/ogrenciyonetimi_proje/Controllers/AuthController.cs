using Microsoft.AspNetCore.Mvc;
using ogrenciyonetimi_proje.Models;
using ogrenciyonetimi_proje.Services;

namespace ogrenciyonetimi_proje.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>Yeni kullanıcı kaydı</summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var (success, message) = await _authService.RegisterAsync(dto);
            if (!success)
                return BadRequest(new { message });
            return Ok(new { message });
        }

        /// <summary>Giriş yap ve JWT token al</summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var (success, token, user) = await _authService.LoginAsync(dto);
            if (!success)
                return Unauthorized(new { message = "E-posta veya şifre hatalı." });

            return Ok(new { token, user });
        }
    }
}
