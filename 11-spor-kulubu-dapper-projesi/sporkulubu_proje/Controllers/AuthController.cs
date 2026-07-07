using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using sporkulubu_proje.Models;
using sporkulubu_proje.Services;

namespace sporkulubu_proje.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (success, msg) = await _auth.RegisterAsync(model);
            if (!success) return BadRequest(new { message = msg });

            return Ok(new { message = msg });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (success, token, user) = await _auth.LoginAsync(model);
            if (!success) return Unauthorized(new { message = "Giriş başarısız. Bilgilerinizi kontrol edin." });

            return Ok(user);
        }
    }
}
