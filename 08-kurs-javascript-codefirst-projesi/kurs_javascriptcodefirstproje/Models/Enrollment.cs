using System.ComponentModel.DataAnnotations;

namespace kurs_javascriptcodefirstproje.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad Soyad zorunludur.")]
        public string StudentName { get; set; }

        [Required(ErrorMessage = "Email zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefon zorunludur.")]
        public string Phone { get; set; }

        public DateTime EnrollmentDate { get; set; }

        public int CourseId { get; set; }

        public Course? Course { get; set; }
    }
}