using System.ComponentModel.DataAnnotations;

namespace kuafor_ORMproje.ViewModels
{
    public class VerifyEmailViewModel
    {
        [Required(ErrorMessage = "email zorunludur")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
