using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using seyahat_projesi.Data;
using seyahat_projesi.Data.Repository.IRepository;
using seyahat_projesi.Model;
using seyahat_projesi.Services;
using seyahat_projesi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace seyahat_projesi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[action]/{id?}")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMemoryCache _cache;
        private readonly LogService _logService;

        public AdminController(
            IUnitOfWork unitOfWork,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IMemoryCache cache,
            LogService logService)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _cache = cache;
            _logService = logService;
        }

        // 1. Admin Index Dashboard
        [HttpGet]
        [Route("~/Admin")]
        public async Task<IActionResult> Index()
        {
            var stats = new AdminStatsViewModel
            {
                UsersCount = _userManager.Users.Count(),
                ToursCount = _unitOfWork.Tour.GetAll(t => t.IsActive).Count(),
                BookingsCount = _unitOfWork.Booking.GetAll().Count(),
                TotalEarnings = _unitOfWork.Payment.GetAll(p => p.Status == "completed").Sum(p => p.Amount)
            };

            return View(stats);
        }

        // 2. Tours List & CRUD
        [HttpGet]
        public IActionResult Tours()
        {
            var tours = _unitOfWork.Tour.GetAll(t => t.IsActive, includeProperties: "Category,Guide");

            ViewBag.Categories = _unitOfWork.Category.GetAll();
            ViewBag.Guides = _unitOfWork.Guide.GetAll();
            return View(tours);
        }

        [HttpPost]
        public async Task<IActionResult> SaveTour(Tour model)
        {
            if (model.Id == 0)
            {
                // Create
                model.IsActive = true;
                _unitOfWork.Tour.Add(model);
                _unitOfWork.Save();

                await _logService.LogActionAsync(_userManager.GetUserId(User), "TOUR_CREATE", "info", $"Yeni tur rotası eklendi: {model.Title} (ID: #{model.Id})");
            }
            else
            {
                // Edit
                var tour = _unitOfWork.Tour.GetFirstOrDefault(t => t.Id == model.Id);
                if (tour != null)
                {
                    tour.Title = model.Title;
                    tour.Description = model.Description;
                    tour.Location = model.Location;
                    tour.StartDate = model.StartDate;
                    tour.EndDate = model.EndDate;
                    tour.DurationDays = model.DurationDays;
                    tour.Price = model.Price;
                    tour.Capacity = model.Capacity;
                    tour.ImageUrl = model.ImageUrl;
                    tour.CategoryId = model.CategoryId;
                    tour.GuideId = model.GuideId;

                    _unitOfWork.Tour.Update(tour);
                    _unitOfWork.Save();

                    await _logService.LogActionAsync(_userManager.GetUserId(User), "TOUR_EDIT", "info", $"Tur rotası güncellendi: {model.Title} (ID: #{model.Id})");
                }
            }

            _cache.Remove("ActiveTours");
            return RedirectToAction("Tours");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTour(int id)
        {
            var tour = _unitOfWork.Tour.GetFirstOrDefault(t => t.Id == id);
            if (tour != null)
            {
                tour.IsActive = false; // Soft delete
                _unitOfWork.Tour.Update(tour);
                _unitOfWork.Save();

                await _logService.LogActionAsync(_userManager.GetUserId(User), "TOUR_DELETE", "warn", $"Tur rotası pasife alındı: {tour.Title} (ID: #{tour.Id})");
                _cache.Remove("ActiveTours");
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Tur bulunamadı." });
        }

        // 3. Bookings List
        [HttpGet]
        public IActionResult Bookings()
        {
            var bookings = _unitOfWork.Booking.GetAll(includeProperties: "Tour,User,Payment")
                .OrderByDescending(b => b.Id);

            return View(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveBooking(int id)
        {
            var userId = _userManager.GetUserId(User);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var booking = _unitOfWork.Booking.GetFirstOrDefault(b => b.Id == id, includeProperties: "Tour,Payment");

                if (booking == null)
                {
                    return Json(new { success = false, message = "Rezervasyon bulunamadı." });
                }

                if (booking.Status == "approved")
                {
                    return Json(new { success = false, message = "Rezervasyon zaten onaylanmış." });
                }

                booking.Status = "approved";
                booking.PaymentStatus = "paid";
                _unitOfWork.Booking.Update(booking);

                if (booking.Payment != null)
                {
                    booking.Payment.Status = "completed";
                    _unitOfWork.Payment.Update(booking.Payment);
                }

                _unitOfWork.Save();
                await transaction.CommitAsync();

                await _logService.LogActionAsync(userId, "BOOKING_APPROVE", "info", $"Rezervasyon onaylandı (Rez ID: #{booking.Id}, Rota: {booking.Tour?.Title})");

                return Json(new { success = true, message = "Rezervasyon başarıyla onaylandı." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = "Onaylama sırasında hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var userId = _userManager.GetUserId(User);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var booking = _unitOfWork.Booking.GetFirstOrDefault(b => b.Id == id, includeProperties: "Tour,Payment");

                if (booking == null)
                {
                    return Json(new { success = false, message = "Rezervasyon bulunamadı." });
                }

                if (booking.Status == "cancelled")
                {
                    return Json(new { success = false, message = "Rezervasyon zaten iptal edilmiş." });
                }

                booking.Status = "cancelled";
                booking.PaymentStatus = "refunded";
                _unitOfWork.Booking.Update(booking);

                if (booking.Payment != null)
                {
                    booking.Payment.Status = "refunded";
                    _unitOfWork.Payment.Update(booking.Payment);
                }

                if (booking.Tour != null)
                {
                    booking.Tour.Capacity += booking.GuestsCount;
                    _unitOfWork.Tour.Update(booking.Tour);
                }

                _unitOfWork.Save();
                await transaction.CommitAsync();

                await _logService.LogActionAsync(userId, "BOOKING_CANCEL", "warn", $"Rezervasyon iptal edildi (Rez ID: #{booking.Id}, Rota: {booking.Tour?.Title})");

                return Json(new { success = true, message = "Rezervasyon başarıyla iptal edildi ve tutar iade edildi." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = "İptal sırasında hata oluştu: " + ex.Message });
            }
        }

        // 4. Users List & Roles Management
        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var roles = await _roleManager.Roles.ToListAsync();

            var userRoles = new Dictionary<string, string>();
            foreach (var user in users)
            {
                var r = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = r.FirstOrDefault() ?? "User";
            }

            ViewBag.UserRoles = userRoles;
            ViewBag.AllRoles = roles;

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRoleAndStatus(string userId, string roleName, string status)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "Kullanıcı bulunamadı." });
            }

            if (user.Email == "admin@gezgin.com")
            {
                return Json(new { success = false, message = "Ana yöneticinin rolü veya durumu değiştirilemez." });
            }

            // Update status
            user.Status = status;
            await _userManager.UpdateAsync(user);

            // Update role
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, roleName);

            await _logService.LogActionAsync(_userManager.GetUserId(User), "USER_MANAGEMENT", "info", $"Kullanıcı güncellendi: {user.Email} (Rol: {roleName}, Durum: {status})");

            return Json(new { success = true });
        }

        // 5. System Logs
        [HttpGet]
        public IActionResult Logs(string? search, string? level)
        {
            var logsList = _unitOfWork.SystemLog.GetAll(includeProperties: "User");

            if (!string.IsNullOrEmpty(search))
            {
                logsList = logsList.Where(l => l.Action.ToLower().Contains(search.ToLower()) || 
                                               (l.Details != null && l.Details.ToLower().Contains(search.ToLower())));
            }

            if (!string.IsNullOrEmpty(level))
            {
                logsList = logsList.Where(l => l.Level.Equals(level, StringComparison.OrdinalIgnoreCase));
            }

            var logs = logsList
                .OrderByDescending(l => l.CreatedAt)
                .Take(200)
                .ToList();

            return View(logs);
        }

        // 6. Cache Management
        [HttpGet]
        public IActionResult Cache()
        {
            var cacheKeys = new[] { "CategoriesList", "ActiveTours" };
            var cachedItems = new List<object>();

            foreach (var key in cacheKeys)
            {
                bool isCached = _cache.TryGetValue(key, out _);
                cachedItems.Add(new
                {
                    Key = key,
                    IsCached = isCached,
                    Expiry = isCached ? "Aktif (Max 5-10 dk)" : "Boş"
                });
            }

            ViewBag.CachedKeys = cachedItems;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ClearSystemCache()
        {
            _cache.Remove("CategoriesList");
            _cache.Remove("ActiveTours");
            await _logService.LogActionAsync(_userManager.GetUserId(User), "CACHE_CLEAR_ALL", "warn", "Tüm sistem önbelleği temizlendi.");
            TempData["SuccessMessage"] = "Sistem önbelleği başarıyla temizlendi.";
            return RedirectToAction("Cache");
        }

        [HttpPost]
        public IActionResult ClearCache(string key)
        {
            _cache.Remove(key);
            _logService.LogActionAsync(_userManager.GetUserId(User), "CACHE_CLEAR", "warn", $"Önbellek temizlendi (Anahtar: {key})").Wait();
            return Json(new { success = true });
        }

        // 7. Categories CRUD
        [HttpGet]
        public IActionResult Categories()
        {
            var categories = _unitOfWork.Category.GetAll();
            return View(categories);
        }

        [HttpPost]
        public async Task<IActionResult> SaveCategory(Category model)
        {
            if (model.Id == 0)
            {
                _unitOfWork.Category.Add(model);
                _unitOfWork.Save();
                await _logService.LogActionAsync(_userManager.GetUserId(User), "CATEGORY_CREATE", "info", $"Yeni kategori eklendi: {model.Name}");
            }
            else
            {
                var category = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == model.Id);
                if (category != null)
                {
                    category.Name = model.Name;
                    category.Description = model.Description;
                    _unitOfWork.Category.Update(category);
                    _unitOfWork.Save();
                    await _logService.LogActionAsync(_userManager.GetUserId(User), "CATEGORY_EDIT", "info", $"Kategori güncellendi: {model.Name}");
                }
            }

            _cache.Remove("CategoriesList");
            return RedirectToAction("Categories");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                // Check if any tour belongs to this category
                bool hasTours = _unitOfWork.Tour.GetAll(t => t.CategoryId == id && t.IsActive).Any();
                if (hasTours)
                {
                    return Json(new { success = false, message = "Bu kategoriye bağlı aktif turlar olduğundan silinemez." });
                }

                _unitOfWork.Category.Remove(category);
                _unitOfWork.Save();
                await _logService.LogActionAsync(_userManager.GetUserId(User), "CATEGORY_DELETE", "warn", $"Kategori silindi: {category.Name}");
                _cache.Remove("CategoriesList");
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Kategori bulunamadı." });
        }

        // 8. Coupons CRUD
        [HttpGet]
        public IActionResult Coupons()
        {
            var coupons = _unitOfWork.Coupon.GetAll();
            return View(coupons);
        }

        [HttpPost]
        public async Task<IActionResult> SaveCoupon(Coupon model)
        {
            model.Code = model.Code.ToUpper();
            if (model.Id == 0)
            {
                _unitOfWork.Coupon.Add(model);
                _unitOfWork.Save();
                await _logService.LogActionAsync(_userManager.GetUserId(User), "COUPON_CREATE", "info", $"Yeni kupon oluşturuldu: {model.Code}");
            }
            else
            {
                var coupon = _unitOfWork.Coupon.GetFirstOrDefault(c => c.Id == model.Id);
                if (coupon != null)
                {
                    coupon.Code = model.Code;
                    coupon.DiscountType = model.DiscountType;
                    coupon.DiscountValue = model.DiscountValue;
                    coupon.ExpiryDate = model.ExpiryDate;
                    coupon.IsActive = model.IsActive;

                    _unitOfWork.Coupon.Update(coupon);
                    _unitOfWork.Save();
                    await _logService.LogActionAsync(_userManager.GetUserId(User), "COUPON_EDIT", "info", $"Kupon güncellendi: {model.Code}");
                }
            }

            return RedirectToAction("Coupons");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var coupon = _unitOfWork.Coupon.GetFirstOrDefault(c => c.Id == id);
            if (coupon != null)
            {
                _unitOfWork.Coupon.Remove(coupon);
                _unitOfWork.Save();
                await _logService.LogActionAsync(_userManager.GetUserId(User), "COUPON_DELETE", "warn", $"Kupon silindi: {coupon.Code}");
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Kupon bulunamadı." });
        }

        // 9. Blogs CRUD
        [HttpGet]
        public IActionResult Blogs()
        {
            var blogs = _unitOfWork.Blog.GetAll();
            return View(blogs);
        }

        [HttpPost]
        public async Task<IActionResult> SaveBlog(Blog model)
        {
            if (model.Id == 0)
            {
                model.CreatedAt = DateTime.Now;
                _unitOfWork.Blog.Add(model);
                _unitOfWork.Save();
                await _logService.LogActionAsync(_userManager.GetUserId(User), "BLOG_CREATE", "info", $"Yeni blog yazısı eklendi: {model.Title}");
            }
            else
            {
                var blog = _unitOfWork.Blog.GetFirstOrDefault(b => b.Id == model.Id);
                if (blog != null)
                {
                    blog.Title = model.Title;
                    blog.Content = model.Content;
                    blog.ImageUrl = model.ImageUrl;

                    _unitOfWork.Blog.Update(blog);
                    _unitOfWork.Save();
                    await _logService.LogActionAsync(_userManager.GetUserId(User), "BLOG_EDIT", "info", $"Blog yazısı güncellendi: {model.Title}");
                }
            }

            return RedirectToAction("Blogs");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var blog = _unitOfWork.Blog.GetFirstOrDefault(b => b.Id == id);
            if (blog != null)
            {
                _unitOfWork.Blog.Remove(blog);
                _unitOfWork.Save();
                await _logService.LogActionAsync(_userManager.GetUserId(User), "BLOG_DELETE", "warn", $"Blog yazısı silindi: {blog.Title}");
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Blog yazısı bulunamadı." });
        }

        // 10. Reviews Moderation
        [HttpGet]
        public IActionResult Reviews()
        {
            var reviews = _unitOfWork.Review.GetAll(includeProperties: "Tour,User")
                .OrderByDescending(r => r.CreatedAt);

            return View(reviews);
        }

        [HttpPost]
        public IActionResult UpdateReviewStatus(int id, string status)
        {
            var review = _unitOfWork.Review.GetFirstOrDefault(r => r.Id == id);
            if (review != null)
            {
                return Json(new { success = false, message = "Defter şemasında durum alanı bulunmamaktadır. Uygunsuz yorumları silebilirsiniz." });
            }
            return Json(new { success = false, message = "Yorum bulunamadı." });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = _unitOfWork.Review.GetFirstOrDefault(r => r.Id == id);
            if (review != null)
            {
                _unitOfWork.Review.Remove(review);
                _unitOfWork.Save();
                await _logService.LogActionAsync(_userManager.GetUserId(User), "REVIEW_DELETE", "warn", $"Yorum silindi (ID: #{review.Id})");
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Yorum bulunamadı." });
        }

        // 11. Contact Messages
        [HttpGet]
        public IActionResult Messages()
        {
            var messages = _unitOfWork.ContactMessage.GetAll()
                .OrderByDescending(m => m.CreatedAt);

            return View(messages);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMessageStatus(int id, bool isRead)
        {
            var msg = _unitOfWork.ContactMessage.GetFirstOrDefault(m => m.Id == id);
            if (msg != null)
            {
                msg.IsRead = isRead;
                _unitOfWork.ContactMessage.Update(msg);
                _unitOfWork.Save();
                await _logService.LogActionAsync(_userManager.GetUserId(User), "MESSAGE_STATUS", "info", $"Mesaj okundu durumu güncellendi (ID: #{msg.Id}, Okundu: {isRead})");
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Mesaj bulunamadı." });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var msg = _unitOfWork.ContactMessage.GetFirstOrDefault(m => m.Id == id);
            if (msg != null)
            {
                _unitOfWork.ContactMessage.Remove(msg);
                _unitOfWork.Save();
                await _logService.LogActionAsync(_userManager.GetUserId(User), "MESSAGE_DELETE", "warn", $"Mesaj silindi (ID: #{msg.Id})");
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Mesaj bulunamadı." });
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(string name, string email, string password, string roleName, string status)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return Json(new { success = false, message = "Ad Soyad, E-posta ve Şifre alanları boş bırakılamaz." });
            }

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return Json(new { success = false, message = "Bu e-posta adresiyle zaten bir kullanıcı kayıtlı." });
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                Name = name,
                Status = status ?? "active",
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Json(new { success = false, message = $"Kullanıcı oluşturulurken hata oluştu: {errors}" });
            }

            var roleResult = await _userManager.AddToRoleAsync(user, roleName ?? "User");
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                return Json(new { success = false, message = $"Kullanıcı rolü atanırken hata oluştu: {errors}" });
            }

            await _logService.LogActionAsync(_userManager.GetUserId(User), "USER_CREATE", "info", $"Yeni kullanıcı oluşturuldu: {user.Email} (Rol: {roleName})");
            return Json(new { success = true, message = "Kullanıcı başarıyla oluşturuldu." });
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(string userId, string name, string email, string? password)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
            {
                return Json(new { success = false, message = "Ad Soyad ve E-posta alanları boş bırakılamaz." });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "Kullanıcı bulunamadı." });
            }

            if (user.Email == "admin@gezgin.com" && email != "admin@gezgin.com")
            {
                return Json(new { success = false, message = "Ana yöneticinin e-posta adresi değiştirilemez." });
            }

            var existingWithEmail = await _userManager.FindByEmailAsync(email);
            if (existingWithEmail != null && existingWithEmail.Id != userId)
            {
                return Json(new { success = false, message = "Bu e-posta adresi başka bir kullanıcı tarafından kullanılıyor." });
            }

            user.Name = name;
            user.Email = email;
            user.UserName = email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Json(new { success = false, message = $"Kullanıcı güncellenirken hata oluştu: {errors}" });
            }

            if (!string.IsNullOrWhiteSpace(password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _userManager.ResetPasswordAsync(user, token, password);
                if (!resetResult.Succeeded)
                {
                    var errors = string.Join(", ", resetResult.Errors.Select(e => e.Description));
                    return Json(new { success = false, message = $"Şifre güncellenirken hata oluştu: {errors}" });
                }
            }

            await _logService.LogActionAsync(_userManager.GetUserId(User), "USER_EDIT", "info", $"Kullanıcı bilgileri güncellendi: {user.Email}");
            return Json(new { success = true, message = "Kullanıcı başarıyla güncellendi." });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == id)
            {
                return Json(new { success = false, message = "Kendi hesabınızı silemezsiniz." });
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = "Kullanıcı bulunamadı." });
            }

            if (user.Email == "admin@gezgin.com")
            {
                return Json(new { success = false, message = "Ana yönetici hesabı silinemez." });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var reviews = _unitOfWork.Review.GetAll(r => r.UserId == id);
                _unitOfWork.Review.RemoveRange(reviews);

                var bookings = _unitOfWork.Booking.GetAll(b => b.UserId == id);
                foreach (var b in bookings)
                {
                    var payment = _unitOfWork.Payment.GetFirstOrDefault(p => p.BookingId == b.Id);
                    if (payment != null)
                    {
                        _unitOfWork.Payment.Remove(payment);
                    }
                    _unitOfWork.Booking.Remove(b);
                }

                _unitOfWork.Save();

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return Json(new { success = false, message = $"Kullanıcı silinirken hata oluştu: {errors}" });
                }

                await transaction.CommitAsync();

                await _logService.LogActionAsync(currentUserId, "USER_DELETE", "warn", $"Kullanıcı silindi: {user.Email}");
                return Json(new { success = true, message = "Kullanıcı başarıyla silindi." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = $"Silme işlemi başarısız: {ex.Message}" });
            }
        }
    }
}
