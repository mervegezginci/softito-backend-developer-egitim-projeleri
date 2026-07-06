using System.ComponentModel.DataAnnotations;

namespace kuafor_ORMproje.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "email zorunludur")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "parola zorunludur")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Hatırla")]
        public bool RememberMe { get; set; }
    }
}
