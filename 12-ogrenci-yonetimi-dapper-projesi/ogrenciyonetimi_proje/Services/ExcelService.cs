using OfficeOpenXml;
using OfficeOpenXml.Style;
using ogrenciyonetimi_proje.Models;
using System.Drawing;

namespace ogrenciyonetimi_proje.Services
{
    public interface IExcelService
    {
        byte[] GenerateStudentListExcel(IEnumerable<Student> students);
    }

    public class ExcelService : IExcelService
    {
        public ExcelService()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public byte[] GenerateStudentListExcel(IEnumerable<Student> students)
        {
            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Öğrenciler");

            // Başlık satırı
            string[] headers = { "ID", "Ad", "Soyad", "E-posta", "Telefon", "Bölüm", "Sınıf", "Aktif", "Kayıt Tarihi" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = sheet.Cells[1, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(41, 128, 185));
                cell.Style.Font.Color.SetColor(Color.White);
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            // Veri satırları
            int row = 2;
            foreach (var s in students)
            {
                sheet.Cells[row, 1].Value = s.StudentId;
                sheet.Cells[row, 2].Value = s.FirstName;
                sheet.Cells[row, 3].Value = s.LastName;
                sheet.Cells[row, 4].Value = s.Email;
                sheet.Cells[row, 5].Value = s.Phone ?? "-";
                sheet.Cells[row, 6].Value = s.DepartmentName ?? "-";
                sheet.Cells[row, 7].Value = s.GradeLevel;
                sheet.Cells[row, 8].Value = s.IsActive ? "Evet" : "Hayır";
                sheet.Cells[row, 9].Value = s.CreatedAt.ToString("dd.MM.yyyy");

                if (row % 2 == 0)
                {
                    using var range = sheet.Cells[row, 1, row, headers.Length];
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(236, 240, 241));
                }
                row++;
            }

            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
            return package.GetAsByteArray();
        }
    }
}
