using Microsoft.AspNetCore.Identity;

namespace sporkulubu_proje.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Role { get; set; } = "User";
    }
}
