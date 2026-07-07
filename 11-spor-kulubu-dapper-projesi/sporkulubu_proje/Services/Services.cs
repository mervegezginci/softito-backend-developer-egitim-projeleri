using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing;
using sporkulubu_proje.Models;

namespace sporkulubu_proje.Services
{
    // --- Auth Service ---

    public interface IAuthService
    {
        Task<(bool Success, string Message)> RegisterAsync(RegisterModel model);
        Task<(bool Success, string Token, LoginResponse? User)> LoginAsync(LoginModel model);
    }

    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                Role = model.Role
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errorMsg = string.Join(" ", result.Errors.Select(e => e.Description));
                return (false, errorMsg);
            }

            return (true, "Kayıt başarılı.");
        }

        public async Task<(bool Success, string Token, LoginResponse? User)> LoginAsync(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return (false, string.Empty, null);
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
            {
                return (false, string.Empty, null);
            }

            var responseUser = new LoginResponse
            {
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Role = user.Role
            };

            var token = GenerateJwtToken(user);
            responseUser.Token = token;

            return (true, token, responseUser);
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
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

    // --- Excel Service ---

    public interface IExcelService
    {
        byte[] GenerateMemberListExcel(IEnumerable<Member> members);
    }

    public class ExcelService : IExcelService
    {
        public ExcelService()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public byte[] GenerateMemberListExcel(IEnumerable<Member> members)
        {
            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Üyeler");

            // Başlık satırı
            string[] headers = { "Üye ID", "Ad", "Soyad", "E-posta", "Telefon", "Kayıt Tarihi", "Durum" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = sheet.Cells[1, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 70, 229)); // Indigo primary theme
                cell.Style.Font.Color.SetColor(Color.White);
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            // Veri satırları
            int row = 2;
            foreach (var m in members)
            {
                sheet.Cells[row, 1].Value = m.MemberId;
                sheet.Cells[row, 2].Value = m.FirstName;
                sheet.Cells[row, 3].Value = m.LastName;
                sheet.Cells[row, 4].Value = m.Email;
                sheet.Cells[row, 5].Value = m.Phone ?? "-";
                sheet.Cells[row, 6].Value = m.JoinDate.ToString("dd.MM.yyyy");
                sheet.Cells[row, 7].Value = m.IsActive ? "Aktif" : "Pasif";

                if (row % 2 == 0)
                {
                    using var range = sheet.Cells[row, 1, row, headers.Length];
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(243, 244, 246));
                }
                row++;
            }

            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
            return package.GetAsByteArray();
        }
    }

    // --- Pdf Service ---

    public interface IPdfService
    {
        byte[] GenerateMemberListPdf(IEnumerable<Member> members);
    }

    public class PdfService : IPdfService
    {
        public byte[] GenerateMemberListPdf(IEnumerable<Member> members)
        {
            using var ms = new MemoryStream();
            var document = new Document(PageSize.A4, 25f, 25f, 30f, 30f);
            PdfWriter.GetInstance(document, ms);
            document.Open();

            // Fontlar
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.DARK_GRAY);
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
            var cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.BLACK);

            document.Add(new Paragraph("Spor Kulübü Üye Listesi", titleFont));
            document.Add(new Paragraph($"Rapor Tarihi: {DateTime.Now:dd.MM.yyyy HH:mm}", cellFont));
            document.Add(new Paragraph(" "));

            // Tablo
            var table = new PdfPTable(6) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 1f, 2.5f, 2.5f, 3.5f, 2.5f, 1.5f });

            var headerBg = new BaseColor(79, 70, 229); // Indigo theme
            string[] headers = { "ID", "Ad", "Soyad", "E-posta", "Telefon", "Durum" };
            foreach (var h in headers)
            {
                var cell = new PdfPCell(new Phrase(h, headerFont))
                {
                    BackgroundColor = headerBg,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 6
                };
                table.AddCell(cell);
            }

            bool alt = false;
            var altBg = new BaseColor(243, 244, 246);
            foreach (var m in members)
            {
                var bg = alt ? altBg : BaseColor.WHITE;
                void AddCell(string text)
                {
                    var c = new PdfPCell(new Phrase(text, cellFont))
                    {
                        BackgroundColor = bg,
                        Padding = 5
                    };
                    table.AddCell(c);
                }
                AddCell(m.MemberId.ToString());
                AddCell(m.FirstName);
                AddCell(m.LastName);
                AddCell(m.Email);
                AddCell(m.Phone ?? "-");
                AddCell(m.IsActive ? "Aktif" : "Pasif");
                alt = !alt;
            }

            document.Add(table);
            document.Close();
            return ms.ToArray();
        }
    }
}
