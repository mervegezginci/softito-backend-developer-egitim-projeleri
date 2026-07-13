using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using seyahat_projesi.Data;
using seyahat_projesi.Model;
using seyahat_projesi.Services;
using seyahat_projesi.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace seyahat_projesi.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly LogService _logService;

        public BookingController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            LogService logService)
        {
            _context = context;
            _userManager = userManager;
            _logService = logService;
        }

        [HttpGet]
        public IActionResult MyBookings()
        {
            return RedirectToAction("Bookings", "Dashboard", new { area = "User" });
        }

        [HttpGet]
        public async Task<IActionResult> Checkout(int tourId)
        {
            var tour = await _context.Tours
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == tourId && t.IsActive);

            if (tour == null)
            {
                TempData["ErrorMessage"] = "Seçilen seyahat rotası bulunamadı veya aktif değil.";
                return RedirectToAction("Index", "Home");
            }

            var userId = _userManager.GetUserId(User);

            // Kontenjan kontrolü
            if (tour.Capacity <= 0)
            {
                TempData["ErrorMessage"] = "Bu turun kontenjanı dolmuştur, rezervasyon yapılamaz.";
                return RedirectToAction("Detail", "Home", new { id = tourId });
            }

            // Mükerrer rezervasyon kontrolü
            var existingBooking = await _context.Bookings
                .AnyAsync(b => b.TourId == tourId && b.UserId == userId && b.Status != "cancelled");

            if (existingBooking)
            {
                TempData["ErrorMessage"] = "Bu tura zaten aktif bir rezervasyonunuz bulunmaktadır.";
                return RedirectToAction("Detail", "Home", new { id = tourId });
            }

            var user = await _userManager.FindByIdAsync(userId!);

            ViewBag.UserFullName = user?.Name ?? "";
            ViewBag.UserEmail = user?.Email ?? "";

            return View(tour);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int tourId, DateTime bookingDate, int guestsCount, string paymentMethod, string cardNumber, string? couponCode)
        {
            var userId = _userManager.GetUserId(User);

            if (guestsCount <= 0)
            {
                TempData["ErrorMessage"] = "Kişi sayısı en az 1 olmalıdır.";
                return RedirectToAction("Index", "Home");
            }

            // Mükerrer rezervasyon kontrolü
            var existingBooking = await _context.Bookings
                .AnyAsync(b => b.TourId == tourId && b.UserId == userId && b.Status != "cancelled");

            if (existingBooking)
            {
                TempData["ErrorMessage"] = "Bu tura zaten aktif bir rezervasyonunuz bulunmaktadır.";
                return RedirectToAction("Index", "Home");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var tour = await _context.Tours.FindAsync(tourId);
                if (tour == null || !tour.IsActive)
                {
                    TempData["ErrorMessage"] = "Tur rotası bulunamadı.";
                    return RedirectToAction("Index", "Home");
                }

                if (tour.Capacity < guestsCount)
                {
                    TempData["ErrorMessage"] = $"Kontenjan yetersiz. Kalan yer: {tour.Capacity}";
                    return RedirectToAction("Index", "Home");
                }

                double basePrice = tour.Price * guestsCount;
                double finalPrice = basePrice;
                int? couponId = null;

                // Validate and apply coupon if provided
                if (!string.IsNullOrEmpty(couponCode))
                {
                    var coupon = await _context.Coupons
                        .FirstOrDefaultAsync(c => c.Code == couponCode.ToUpper() && c.IsActive && c.ExpiryDate >= DateTime.Now);
                    
                    if (coupon != null)
                    {
                        couponId = coupon.Id;
                        if (coupon.DiscountType == "percentage")
                        {
                            finalPrice = basePrice * (1 - (coupon.DiscountValue / 100.0));
                        }
                        else
                        {
                            finalPrice = Math.Max(0, basePrice - coupon.DiscountValue);
                        }
                    }
                }

                // Create Booking
                var booking = new Booking
                {
                    UserId = userId!,
                    TourId = tourId,
                    BookingDate = bookingDate,
                    GuestsCount = guestsCount,
                    TotalPrice = finalPrice,
                    PaymentStatus = "paid", // Instantly paid due to simulated transaction
                    Status = "approved",
                    CouponId = couponId
                };
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                // Generate Mock Transaction ID
                string mockTransactionId = "TXN-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

                // Create Payment
                var payment = new Payment
                {
                    BookingId = booking.Id,
                    PaymentMethod = paymentMethod,
                    TransactionId = mockTransactionId,
                    Amount = finalPrice,
                    Status = "completed",
                    PaymentDate = DateTime.Now
                };
                _context.Payments.Add(payment);

                // Decrease Tour Capacity
                tour.Capacity -= guestsCount;
                _context.Tours.Update(tour);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                await _logService.LogActionAsync(userId, "BOOKING_CREATE", "info", $"Yeni rezervasyon oluşturuldu (Rez ID: #{booking.Id}, Rota: {tour.Title}, Tutar: {finalPrice} TL)");
                await _logService.LogActionAsync(userId, "PAYMENT_RECEIVE", "info", $"Sanal ödeme alındı (Tutar: {finalPrice} TL, Yöntem: {paymentMethod}, İşlem ID: {mockTransactionId})");

                TempData["SuccessMessage"] = "Rezervasyonunuz ve ödeme işleminiz başarıyla tamamlandı!";
                return RedirectToAction("Success", new { id = booking.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Rezervasyon oluşturulurken teknik bir hata oluştu: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Success(int id)
        {
            var userId = _userManager.GetUserId(User);
            var booking = await _context.Bookings
                .Include(b => b.Tour)
                .Include(b => b.Payment)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(booking);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Tour)
                    .Include(b => b.Payment)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (booking == null)
                {
                    return Json(new { success = false, message = "Rezervasyon bulunamadı." });
                }

                if (booking.UserId != userId && !isAdmin)
                {
                    return Json(new { success = false, message = "Bu işlemi yapmak için yetkiniz bulunmamaktadır." });
                }

                if (booking.Status == "cancelled")
                {
                    return Json(new { success = false, message = "Rezervasyon zaten iptal edilmiş." });
                }

                // Update booking status
                booking.Status = "cancelled";
                booking.PaymentStatus = "refunded";
                _context.Bookings.Update(booking);

                // Update payment status
                if (booking.Payment != null)
                {
                    booking.Payment.Status = "refunded";
                    _context.Payments.Update(booking.Payment);
                }

                // Restore Tour capacity
                if (booking.Tour != null)
                {
                    booking.Tour.Capacity += booking.GuestsCount;
                    _context.Tours.Update(booking.Tour);
                }

                await _context.SaveChangesAsync();
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var userId = _userManager.GetUserId(User);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Tour)
                    .Include(b => b.Payment)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (booking == null)
                {
                    return Json(new { success = false, message = "Rezervasyon bulunamadı." });
                }

                if (booking.Status == "approved")
                {
                    return Json(new { success = false, message = "Rezervasyon zaten onaylanmış." });
                }

                // Update booking status
                booking.Status = "approved";
                booking.PaymentStatus = "paid";
                _context.Bookings.Update(booking);

                // Update payment status
                if (booking.Payment != null)
                {
                    booking.Payment.Status = "completed";
                    _context.Payments.Update(booking.Payment);
                }

                await _context.SaveChangesAsync();
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
    }
}
