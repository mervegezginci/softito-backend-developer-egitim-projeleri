using Microsoft.AspNetCore.Identity;

namespace seyahat_projesi.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = "active"; // active, suspended
    }
}
