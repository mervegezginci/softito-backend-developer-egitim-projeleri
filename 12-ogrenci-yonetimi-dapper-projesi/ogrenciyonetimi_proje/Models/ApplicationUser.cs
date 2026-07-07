using Microsoft.AspNetCore.Identity;

namespace ogrenciyonetimi_proje.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Role { get; set; } = "User";
    }
}
