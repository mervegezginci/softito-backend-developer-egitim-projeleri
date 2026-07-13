using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using seyahat_projesi.Model;
using seyahat_projesi.Services;
using seyahat_projesi.ViewModels;
using System.Threading.Tasks;

namespace seyahat_projesi.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly LogService _logService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            LogService logService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logService = logService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "E-posta veya şifre hatalı.");
                    await _logService.LogActionAsync(null, "AUTH_LOGIN_FAILED", "warn", $"Başarısız giriş denemesi (Kullanıcı bulunamadı): {model.Email}");
                    return View(model);
                }

                if (user.Status == "suspended")
                {
                    ModelState.AddModelError("", "Hesabınız askıya alınmıştır. Lütfen destek ile iletişime geçin.");
                    await _logService.LogActionAsync(user.Id, "AUTH_LOGIN_FAILED", "warn", $"Askıya alınmış hesaba giriş denemesi: {model.Email}");
                    return View(model);
                }

                // Password check using SignInManager
                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName!, 
                    model.Password, 
                    model.RememberMe, 
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    await _logService.LogActionAsync(user.Id, "AUTH_LOGIN_SUCCESS", "info", $"Kullanıcı başarıyla giriş yaptı: {model.Email}");
                    
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "E-posta veya şifre hatalı.");
                await _logService.LogActionAsync(user.Id, "AUTH_LOGIN_FAILED", "warn", $"Başarısız giriş denemesi (Hatalı şifre): {model.Email}");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Bu e-posta adresi zaten kullanımda.");
                    return View(model);
                }

                var user = new ApplicationUser
                {
                    Name = model.Name,
                    UserName = model.Email,
                    Email = model.Email,
                    Status = "active"
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Add default role
                    await _userManager.AddToRoleAsync(user, "User");

                    await _logService.LogActionAsync(user.Id, "USER_REGISTER", "info", $"Yeni kullanıcı kayıt oldu: {model.Email}");

                    TempData["SuccessMessage"] = "Kayıt işleminiz başarılı! Şimdi giriş yapabilirsiniz.";
                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var userId = _userManager.GetUserId(User);
            await _signInManager.SignOutAsync();

            if (userId != null)
            {
                await _logService.LogActionAsync(userId, "AUTH_LOGOUT", "info", "Kullanıcı çıkış yaptı.");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Lütfen e-posta adresinizi giriniz.");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                await _logService.LogActionAsync(user.Id, "AUTH_FORGOT_PASSWORD_REQUEST", "info", $"Şifre sıfırlama talebi alındı: {email}");
            }

            TempData["SuccessMessage"] = "Şifre sıfırlama bağlantısı e-posta adresinize başarıyla gönderildi! Lütfen kutunuzu kontrol edin.";
            return RedirectToAction("Login");
        }
    }
}
