using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using seyahat_projesi.Data;
using seyahat_projesi.Model;
using seyahat_projesi.Services;
using seyahat_projesi.ViewModels;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace seyahat_projesi.Areas.User.Controllers
{
    [Area("User")]
    [Route("User")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly LogService _logService;

        public DashboardController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            LogService logService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logService = logService;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId!);

            var bookings = await _context.Bookings
                .Include(b => b.Tour)
                .Include(b => b.Payment)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.Id)
                .ToListAsync();

            // Calculate stats
            ViewBag.ActiveBookingsCount = bookings.Count(b => b.Status == "approved" && b.Tour.StartDate > DateTime.Now);
            ViewBag.CompletedTripsCount = bookings.Count(b => b.Status == "approved" && b.Tour.EndDate <= DateTime.Now);
            ViewBag.TotalSpent = bookings.Where(b => b.PaymentStatus == "paid").Sum(b => b.TotalPrice);

            var viewModel = new DashboardViewModel
            {
                User = user!,
                Bookings = bookings.Take(5).ToList() // Show top 5 bookings on dashboard
            };

            return View(viewModel);
        }

        [HttpGet("Bookings")]
        public async Task<IActionResult> Bookings()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId!);

            var bookings = await _context.Bookings
                .Include(b => b.Tour)
                .Include(b => b.Payment)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.Id)
                .ToListAsync();

            var viewModel = new DashboardViewModel
            {
                User = user!,
                Bookings = bookings
            };

            return View(viewModel);
        }

        [HttpGet("Profile")]
        public async Task<IActionResult> Profile()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId!);
            return View(user);
        }

        [HttpPost("Profile")]
        public async Task<IActionResult> Profile(string name, string email)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId!);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Profile");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["ErrorMessage"] = "Ad Soyad alanı boş bırakılamaz.";
                return View(user);
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["ErrorMessage"] = "E-posta adresi boş bırakılamaz.";
                return View(user);
            }

            // E-posta benzersizlik kontrolü
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null && existingUser.Id != user.Id)
            {
                TempData["ErrorMessage"] = "Bu e-posta adresi başka bir kullanıcı tarafından kullanılmaktadır.";
                return View(user);
            }

            user.Name = name;
            user.Email = email;
            user.UserName = email;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _logService.LogActionAsync(userId, "PROFILE_UPDATE", "info", $"Kullanıcı profil bilgilerini güncelledi.");
                TempData["SuccessMessage"] = "Profil bilgileriniz başarıyla güncellendi.";
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToAction("Profile");
            }

            TempData["ErrorMessage"] = "Profil güncellenirken bir hata oluştu: " + string.Join(", ", result.Errors.Select(e => e.Description));
            return View(user);
        }

        [HttpPost("Cancel/{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = _userManager.GetUserId(User);
            var booking = await _context.Bookings
                .Include(b => b.Tour)
                .Include(b => b.Payment)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null)
            {
                return Json(new { success = false, message = "İptal edilmek istenen rezervasyon bulunamadı." });
            }

            if (booking.Status == "cancelled")
            {
                return Json(new { success = false, message = "Bu rezervasyon zaten iptal edilmiş." });
            }

            if (booking.Tour != null && booking.Tour.StartDate <= DateTime.Now.AddDays(3))
            {
                return Json(new { success = false, message = "Seyahate 3 günden az süre kala iptal işlemi yapılamaz." });
            }

            // Restore Tour Capacity
            if (booking.Tour != null)
            {
                booking.Tour.Capacity += booking.GuestsCount;
            }
            booking.Status = "cancelled";
            booking.PaymentStatus = "refunded";

            if (booking.Payment != null)
            {
                booking.Payment.Status = "refunded";
            }

            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();

            await _logService.LogActionAsync(userId, "BOOKING_CANCEL", "warning", $"Rezervasyon iptal edildi ve iade süreci başlatıldı (Rez ID: #{booking.Id}, Tur: {booking.Tour?.Title})");

            return Json(new { success = true, message = "Rezervasyonunuz başarıyla iptal edildi ve ödemeniz iade edildi." });
        }

        [HttpGet("GetRandomCoupon")]
        public async Task<IActionResult> GetRandomCoupon()
        {
            var coupon = await _context.Coupons
                .Where(c => c.IsActive && c.ExpiryDate > DateTime.Now)
                .OrderBy(r => Guid.NewGuid())
                .FirstOrDefaultAsync();

            if (coupon == null)
            {
                return NotFound(new { message = "Aktif kupon bulunamadı." });
            }

            string localIp = "localhost";
            try
            {
                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus == OperationalStatus.Up && 
                        (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || 
                         ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet))
                    {
                        foreach (var ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                string name = ni.Name.ToLower();
                                string desc = ni.Description.ToLower();
                                if (!name.Contains("virtual") && !desc.Contains("virtual") && 
                                    !name.Contains("vbox") && !desc.Contains("vbox") &&
                                    !name.Contains("vmware") && !desc.Contains("vmware") &&
                                    !name.Contains("wsl") && !desc.Contains("wsl") &&
                                    !name.Contains("loopback") && !desc.Contains("loopback"))
                                {
                                    localIp = ip.Address.ToString();
                                    break;
                                }
                            }
                        }
                    }
                    if (localIp != "localhost") break;
                }

                // Fallback to DNS if still localhost
                if (localIp == "localhost")
                {
                    var hostName = System.Net.Dns.GetHostName();
                    var hostEntry = System.Net.Dns.GetHostEntry(hostName);
                    foreach (var ip in hostEntry.AddressList)
                    {
                        if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !ip.ToString().StartsWith("127."))
                        {
                            localIp = ip.ToString();
                            break;
                        }
                    }
                }
            }
            catch { }

            string scheme = Request.Scheme;
            string hostStr;

            var env = HttpContext.RequestServices.GetService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();
            if (env != null && env.EnvironmentName == "Development")
            {
                // For development local network testing, use HTTP to avoid self-signed SSL/HTTPS warning blocks on mobile devices
                scheme = "http";
                int httpPort = 5256; // default fallback

                var server = HttpContext.RequestServices.GetService<Microsoft.AspNetCore.Hosting.Server.IServer>();
                var addresses = server?.Features.Get<Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature>()?.Addresses;
                if (addresses != null)
                {
                    foreach (var address in addresses)
                    {
                        if (address.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                        {
                            if (Uri.TryCreate(address, UriKind.Absolute, out var uri))
                            {
                                httpPort = uri.Port;
                                break;
                            }
                        }
                    }
                }
                hostStr = $"{localIp}:{httpPort}";
            }
            else
            {
                hostStr = Request.Host.Port.HasValue ? $"{localIp}:{Request.Host.Port.Value}" : localIp;
            }

            var revealUrl = $"{scheme}://{hostStr}/User/RevealCoupon?code={coupon.Code}";

            return Json(new 
            { 
                code = coupon.Code, 
                discountType = coupon.DiscountType == "percentage" ? "Oransal" : "Sabit",
                discountValue = coupon.DiscountValue,
                revealUrl = revealUrl
            });
        }

        [AllowAnonymous]
        [HttpGet("RevealCoupon")]
        public async Task<IActionResult> RevealCoupon(string code)
        {
            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code == code);

            if (coupon == null)
            {
                return NotFound("Geçersiz kupon kodu.");
            }

            // Mark coupon as scanned/revealed in memory cache
            var cache = HttpContext.RequestServices.GetService<Microsoft.Extensions.Caching.Memory.IMemoryCache>();
            if (cache != null)
            {
                cache.Set($"CouponScanned_{code}", true, TimeSpan.FromMinutes(10));
            }

            return View(coupon);
        }

        [AllowAnonymous]
        [HttpGet("CheckCouponStatus")]
        public IActionResult CheckCouponStatus(string code)
        {
            var cache = HttpContext.RequestServices.GetService<Microsoft.Extensions.Caching.Memory.IMemoryCache>();
            bool isScanned = false;
            if (cache != null)
            {
                isScanned = cache.TryGetValue<bool>($"CouponScanned_{code}", out bool scanned) && scanned;
            }
            return Json(new { scanned = isScanned });
        }
    }
}
