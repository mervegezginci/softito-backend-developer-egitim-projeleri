using kutuphane_apiproje.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace kutuphane_apiproje.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? search)
        {
            ViewBag.Search = search;

            var users = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                users = users.Where(u =>
                    u.Name.ToLower().Contains(search) ||
                    u.Email.ToLower().Contains(search));
            }

            var userList = users.OrderBy(u => u.Name).ToList();

            // Her kullanıcının rolünü al
            var result = new List<(ApplicationUser User, IList<string> Roles)>();
            foreach (var user in userList)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add((user, roles));
            }

            return View(result);
        }

        // Kullanıcıyı Admin Yap
        [HttpPost]
        public async Task<IActionResult> MakeAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && !await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            return RedirectToAction(nameof(Index));
        }

        // Admin rolünü Kaldır
        [HttpPost]
        public async Task<IActionResult> RemoveAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
