using System.ComponentModel.DataAnnotations;

namespace kutuphane_apiproje.ViewModels
{
    public class VerifyEmailViewModel
    {
        [Required(ErrorMessage = "email zorunludur")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
