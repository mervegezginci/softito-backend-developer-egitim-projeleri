using System.ComponentModel.DataAnnotations;

namespace kutuphane_apiproje.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "parola zorunludur")]
        [DataType(DataType.Password)]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "min 8 karakter")]
        [Compare("ConfirmNewPassword", ErrorMessage = "Parola eşleşmedi")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "parola zorunludur")]
        [DataType(DataType.Password)]
        [Display(Name = "yeni parola eşleşme")]
        public string ConfirmNewPassword { get; set; }
    }
}
