using System;
using System.ComponentModel.DataAnnotations;

namespace sporkulubu_mvc.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçersiz e-posta formatı.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçersiz e-posta formatı.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [MinLength(4, ErrorMessage = "Şifre en az 4 karakter olmalıdır.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = "User";
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class BranchViewModel
    {
        public int BranchId { get; set; }

        [Required(ErrorMessage = "Branş adı zorunludur.")]
        [Display(Name = "Spor Branş Adı")]
        public string BranchName { get; set; } = string.Empty;

        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }

    public class CoachViewModel
    {
        public int CoachId { get; set; }

        [Required(ErrorMessage = "Ad zorunludur.")]
        [Display(Name = "Adı")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur.")]
        [Display(Name = "Soyadı")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçersiz e-posta formatı.")]
        [Display(Name = "E-posta Adresi")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Telefon Numarası")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Spor branşı seçimi zorunludur.")]
        [Display(Name = "Spor Branşı")]
        public int BranchId { get; set; }

        public string? BranchName { get; set; } // Join result
    }

    public class MemberViewModel
    {
        public int MemberId { get; set; }

        [Required(ErrorMessage = "Ad zorunludur.")]
        [Display(Name = "Adı")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur.")]
        [Display(Name = "Soyadı")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçersiz e-posta formatı.")]
        [Display(Name = "E-posta Adresi")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Telefon Numarası")]
        public string? Phone { get; set; }

        [Display(Name = "Kayıt Tarihi")]
        public DateTime JoinDate { get; set; } = DateTime.Now;

        [Display(Name = "Durum (Aktif mi?)")]
        public bool IsActive { get; set; } = true;
    }

    public class TrainingViewModel
    {
        public int TrainingId { get; set; }

        [Required(ErrorMessage = "Üye seçimi zorunludur.")]
        [Display(Name = "Kulüp Üyesi")]
        public int MemberId { get; set; }
        public string? MemberName { get; set; } // Join result

        [Required(ErrorMessage = "Antrenör seçimi zorunludur.")]
        [Display(Name = "Sorumlu Antrenör")]
        public int CoachId { get; set; }
        public string? CoachName { get; set; } // Join result

        [Required(ErrorMessage = "Antrenman tarihi ve saati zorunludur.")]
        [Display(Name = "Antrenman Tarihi")]
        public DateTime TrainingDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Seans süresi zorunludur.")]
        [Range(15, 480, ErrorMessage = "Seans süresi 15 ile 480 dakika arasında olmalıdır.")]
        [Display(Name = "Seans Süresi (Dakika)")]
        public int DurationMinutes { get; set; } = 60;

        [Required(ErrorMessage = "Seans ücreti zorunludur.")]
        [Range(0, 100000, ErrorMessage = "Seans ücreti sıfırdan küçük olamaz.")]
        [Display(Name = "Seans Ücreti (TL)")]
        public decimal Fee { get; set; } = 0.00m;

        public string? BranchName { get; set; } // Join result
    }
}
