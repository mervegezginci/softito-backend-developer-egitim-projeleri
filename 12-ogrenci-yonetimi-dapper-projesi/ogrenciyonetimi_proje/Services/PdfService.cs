using iTextSharp.text;
using iTextSharp.text.pdf;
using ogrenciyonetimi_proje.Models;

namespace ogrenciyonetimi_proje.Services
{
    public interface IPdfService
    {
        byte[] GenerateStudentListPdf(IEnumerable<Student> students);
    }

    public class PdfService : IPdfService
    {
        public byte[] GenerateStudentListPdf(IEnumerable<Student> students)
        {
            using var ms = new MemoryStream();
            var document = new Document(PageSize.A4, 25f, 25f, 30f, 30f);
            PdfWriter.GetInstance(document, ms);
            document.Open();

            // Başlık (Türkçe karakter desteği için Arial fontunu yüklüyoruz)
            string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Fonts", "arial.ttf");
            iTextSharp.text.Font titleFont;
            iTextSharp.text.Font headerFont;
            iTextSharp.text.Font cellFont;

            if (File.Exists(fontPath))
            {
                BaseFont bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                titleFont = new iTextSharp.text.Font(bf, 16, iTextSharp.text.Font.BOLD, BaseColor.DARK_GRAY);
                headerFont = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                cellFont = new iTextSharp.text.Font(bf, 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            }
            else
            {
                titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.DARK_GRAY);
                headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
                cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.BLACK);
            }

            document.Add(new Paragraph("Öğrenci Listesi", titleFont));
            document.Add(new Paragraph($"Oluşturulma: {DateTime.Now:dd.MM.yyyy HH:mm}", cellFont));
            document.Add(new Paragraph(" "));

            // Tablo
            var table = new PdfPTable(6) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 1f, 2.5f, 2.5f, 3f, 2f, 1.5f });

            var headerBg = new BaseColor(41, 128, 185);
            string[] headers = { "ID", "Ad", "Soyad", "E-posta", "Bölüm", "Sınıf" };
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
            var altBg = new BaseColor(236, 240, 241);
            foreach (var s in students)
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
                AddCell(s.StudentId.ToString());
                AddCell(s.FirstName);
                AddCell(s.LastName);
                AddCell(s.Email);
                AddCell(s.DepartmentName ?? "-");
                AddCell(s.GradeLevel.ToString());
                alt = !alt;
            }

            document.Add(table);
            document.Close();
            return ms.ToArray();
        }
    }
}
