using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using seyahat_projesi.Data;
using seyahat_projesi.Model;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace seyahat_projesi.Controllers
{
    [Authorize]
    public class ExportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DapperRepository _dapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExportController(ApplicationDbContext context, DapperRepository dapper, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _dapper = dapper;
            _userManager = userManager;
        }

        // 1. Download Booking Invoice as PDF
        [HttpGet]
        public async Task<IActionResult> Invoice(int id)
        {
            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            var booking = await _context.Bookings
                .Include(b => b.Tour)
                .Include(b => b.Payment)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound("Rezervasyon bulunamadı.");
            }

            if (booking.UserId != userId && !isAdmin)
            {
                return Forbid("Bu faturayı indirme yetkiniz bulunmamaktadır.");
            }

            try
            {
                // Create PDF Document using PdfSharp
                using var document = new PdfDocument();
                document.Info.Title = $"Fatura - Rezervasyon #{booking.Id}";

                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);

                // Fonts definition (Arial is standard and supports Turkish character glyphs)
                var fontTitle = new XFont("Arial", 20, XFontStyleEx.Bold);
                var fontSubtitle = new XFont("Arial", 12, XFontStyleEx.Italic);
                var fontHeader = new XFont("Arial", 11, XFontStyleEx.Bold);
                var fontBody = new XFont("Arial", 10, XFontStyleEx.Regular);
                var fontBodyBold = new XFont("Arial", 10, XFontStyleEx.Bold);
                var fontFooter = new XFont("Arial", 8, XFontStyleEx.Regular);

                // Header section
                gfx.DrawString("GM SEYAHAT ACENTESİ", fontTitle, XBrushes.DarkBlue, new XPoint(50, 60));
                gfx.DrawString("Online Rezervasyon Belgesi & Faturası", fontSubtitle, XBrushes.Gray, new XPoint(50, 80));

                // Horizontal separator line
                gfx.DrawLine(XPens.LightGray, 50, 100, 550, 100);

                // Columns for client/invoice details
                gfx.DrawString($"Fatura No: #INV-{booking.Id}", fontBodyBold, XBrushes.Black, new XPoint(50, 130));
                gfx.DrawString($"Tarih: {booking.BookingDate.ToString("dd.MM.yyyy")}", fontBody, XBrushes.Black, new XPoint(50, 150));
                gfx.DrawString($"Ödeme Tipi: {(booking.Payment?.PaymentMethod == "credit_card" ? "Kredi Kartı" : booking.Payment?.PaymentMethod ?? "Banka EFT")}", fontBody, XBrushes.Black, new XPoint(50, 170));

                gfx.DrawString("Müşteri Bilgileri:", fontBodyBold, XBrushes.Black, new XPoint(320, 130));
                gfx.DrawString($"İsim: {booking.User?.Name}", fontBody, XBrushes.DarkSlateGray, new XPoint(320, 150));
                gfx.DrawString($"E-posta: {booking.User?.Email}", fontBody, XBrushes.DarkSlateGray, new XPoint(320, 170));

                // Table Draw
                // Table Background header
                gfx.DrawRectangle(XPens.Gray, XBrushes.AliceBlue, new XRect(50, 210, 500, 25));
                gfx.DrawString("Rota / Tur Detayı", fontHeader, XBrushes.Black, new XPoint(60, 226));
                gfx.DrawString("Seyahat Tarihi", fontHeader, XBrushes.Black, new XPoint(260, 226));
                gfx.DrawString("Kişi", fontHeader, XBrushes.Black, new XPoint(380, 226));
                gfx.DrawString("Toplam Fiyat", fontHeader, XBrushes.Black, new XPoint(460, 226));

                // Table content box
                gfx.DrawRectangle(XPens.LightGray, new XRect(50, 235, 500, 50));
                gfx.DrawString(booking.Tour?.Title ?? "Bilinmeyen Rota", fontBody, XBrushes.DarkSlateGray, new XPoint(60, 255));
                gfx.DrawString(booking.Tour?.Location ?? "", fontSubtitle, XBrushes.Gray, new XPoint(60, 272));
                
                gfx.DrawString(booking.BookingDate.ToString("dd.MM.yyyy"), fontBody, XBrushes.DarkSlateGray, new XPoint(260, 260));
                gfx.DrawString($"{booking.GuestsCount} Kişi", fontBody, XBrushes.DarkSlateGray, new XPoint(380, 260));
                gfx.DrawString($"{booking.TotalPrice.ToString("N0")} TL", fontBody, XBrushes.DarkSlateGray, new XPoint(460, 260));

                // Booking and payment summary
                gfx.DrawString("Rezervasyon Durumu:", fontBodyBold, XBrushes.Black, new XPoint(50, 320));
                
                var statusText = booking.Status == "approved" ? "ONAYLANDI (Ödendi)" : (booking.Status == "cancelled" ? "İPTAL EDİLDİ" : "BEKLEMEDE");
                var statusBrush = booking.Status == "approved" ? XBrushes.Green : XBrushes.Red;
                gfx.DrawString(statusText, fontBodyBold, statusBrush, new XPoint(180, 320));

                gfx.DrawString("Net Toplam:", fontBodyBold, XBrushes.Black, new XPoint(360, 320));
                gfx.DrawString($"{booking.TotalPrice.ToString("N0")} TL", fontBodyBold, XBrushes.Black, new XPoint(460, 320));

                // Bottom Footer disclaimer
                gfx.DrawLine(XPens.LightGray, 50, 360, 550, 360);
                gfx.DrawString("GM Seyahati tercih ettiğiniz için teşekkür ederiz! Tur rehberiniz hareket saatinden önce sizinle iletişime geçecektir.", fontFooter, XBrushes.Gray, new XPoint(50, 385));

                using var stream = new MemoryStream();
                document.Save(stream);
                var content = stream.ToArray();

                return File(content, "application/pdf", $"Bilet-Fatura-{booking.Id}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest($"PDF Raporu oluşturulurken hata oluştu: {ex.Message}");
            }
        }

        // 2. Download Booking Report Excel (Admin only, using Dapper!)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> BookingsReport()
        {
            try
            {
                var sql = @"SELECT b.Id, b.BookingDate, b.GuestsCount, b.TotalPrice, b.Status,
                                   u.Name, u.Email,
                                   t.Title,
                                   p.PaymentMethod, p.TransactionId, p.Status AS PaymentStatus
                            FROM Bookings b
                            LEFT JOIN AspNetUsers u ON b.UserId = u.Id
                            LEFT JOIN Tours t ON b.TourId = t.Id
                            LEFT JOIN Payments p ON b.Id = p.BookingId
                            ORDER BY b.Id DESC";

                var bookings = await _dapper.QueryAsync<dynamic>(sql);

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Rezervasyonlar");

                // Headers
                worksheet.Cell(1, 1).Value = "Rezervasyon ID";
                worksheet.Cell(1, 2).Value = "Müşteri Adı";
                worksheet.Cell(1, 3).Value = "Müşteri E-posta";
                worksheet.Cell(1, 4).Value = "Rota / Tur";
                worksheet.Cell(1, 5).Value = "Gidiş Tarihi";
                worksheet.Cell(1, 6).Value = "Katılımcı Sayısı";
                worksheet.Cell(1, 7).Value = "Toplam Tutar (TL)";
                worksheet.Cell(1, 8).Value = "Rezervasyon Durumu";
                worksheet.Cell(1, 9).Value = "Ödeme Tipi";
                worksheet.Cell(1, 10).Value = "İşlem ID";
                worksheet.Cell(1, 11).Value = "Ödeme Durumu";

                // Format Header row
                var headerRange = worksheet.Range("A1:K1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.AliceBlue;
                headerRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                int row = 2;
                foreach (var b in bookings)
                {
                    worksheet.Cell(row, 1).Value = (int)b.Id;
                    worksheet.Cell(row, 2).Value = (string)(b.Name ?? "Misafir");
                    worksheet.Cell(row, 3).Value = (string)(b.Email ?? "");
                    worksheet.Cell(row, 4).Value = (string)(b.Title ?? "Bilinmeyen Rota");
                    worksheet.Cell(row, 5).Value = ((DateTime)b.BookingDate).ToString("dd.MM.yyyy");
                    worksheet.Cell(row, 6).Value = (int)b.GuestsCount;
                    worksheet.Cell(row, 7).Value = (double)b.TotalPrice;
                    worksheet.Cell(row, 8).Value = (string)b.Status == "approved" ? "Onaylandı" : ((string)b.Status == "cancelled" ? "İptal Edildi" : "Beklemede");
                    worksheet.Cell(row, 9).Value = (string)(b.PaymentMethod == "credit_card" ? "Kredi Kartı" : b.PaymentMethod ?? "Yok");
                    worksheet.Cell(row, 10).Value = (string)(b.TransactionId ?? "Yok");
                    worksheet.Cell(row, 11).Value = (string)(b.PaymentStatus ?? "Yok");
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Rezervasyonlar-Rapor.xlsx");
            }
            catch (Exception ex)
            {
                return BadRequest($"Excel raporu oluşturulamadı: {ex.Message}");
            }
        }

        // 3. Download System Logs Excel (Admin only, using Dapper!)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> LogsReport()
        {
            try
            {
                var sql = @"SELECT l.Id, l.Action, l.Level, l.IpAddress, l.Details, l.CreatedAt,
                                   u.Email
                            FROM SystemLogs l
                            LEFT JOIN AspNetUsers u ON l.UserId = u.Id
                            ORDER BY l.Id DESC";

                var logs = await _dapper.QueryAsync<dynamic>(sql);

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Sistem Logları");

                // Headers
                worksheet.Cell(1, 1).Value = "Log ID";
                worksheet.Cell(1, 2).Value = "Kullanıcı E-posta";
                worksheet.Cell(1, 3).Value = "Aksiyon";
                worksheet.Cell(1, 4).Value = "Seviye";
                worksheet.Cell(1, 5).Value = "IP Adresi";
                worksheet.Cell(1, 6).Value = "Detaylar";
                worksheet.Cell(1, 7).Value = "Zaman Damgası";

                var headerRange = worksheet.Range("A1:G1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                int row = 2;
                foreach (var l in logs)
                {
                    worksheet.Cell(row, 1).Value = (int)l.Id;
                    worksheet.Cell(row, 2).Value = (string)(l.Email ?? "Ziyaretçi");
                    worksheet.Cell(row, 3).Value = (string)l.Action;
                    worksheet.Cell(row, 4).Value = ((string)l.Level).ToUpper();
                    worksheet.Cell(row, 5).Value = (string)l.IpAddress;
                    worksheet.Cell(row, 6).Value = (string)l.Details;
                    worksheet.Cell(row, 7).Value = ((DateTime)l.CreatedAt).ToString("dd.MM.yyyy HH:mm:ss");
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Sistem-Log-Rapor.xlsx");
            }
            catch (Exception ex)
            {
                return BadRequest($"Excel raporu oluşturulamadı: {ex.Message}");
            }
        }
    }
}
