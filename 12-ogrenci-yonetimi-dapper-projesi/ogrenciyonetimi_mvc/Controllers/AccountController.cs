using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ogrenciyonetimi_mvc.Models;
using ogrenciyonetimi_mvc.Services;

namespace ogrenciyonetimi_mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiService _api;

        public AccountController(ApiService api)
        {
            _api = api;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (success, message, response) = await _api.LoginAsync(model);
            if (!success || response == null)
            {
                ModelState.AddModelError("", message);
                return View(model);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, response.User.UserId.ToString()),
                new(ClaimTypes.Name, response.User.Username),
                new(ClaimTypes.Email, response.User.Email),
                new(ClaimTypes.Role, response.User.Role),
                new("JWToken", response.Token) // API çağrıları için token'ı claim içine saklıyoruz
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Student");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (success, message) = await _api.RegisterAsync(model);
            if (!success)
            {
                ModelState.AddModelError("", message);
                return View(model);
            }

            TempData["Success"] = "Kayıt başarılı! Giriş yapabilirsiniz.";
            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}
