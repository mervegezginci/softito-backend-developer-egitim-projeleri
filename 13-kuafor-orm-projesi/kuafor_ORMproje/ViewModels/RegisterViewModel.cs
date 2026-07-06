using System.ComponentModel.DataAnnotations;

namespace kuafor_ORMproje.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "isim zorunludur")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "parola zorunludur")]
        [DataType(DataType.Password)]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "min 8 karakter")]
        [Compare("ConfirmNewPassword", ErrorMessage = "Parola eşleşmedi")]
        public string Password { get; set; }

        [Required(ErrorMessage = "parola zorunludur")]
        [DataType(DataType.Password)]
        [Display(Name = "yeni parola eşleşme")]
        public string ConfirmNewPassword { get; set; }
    }
}
