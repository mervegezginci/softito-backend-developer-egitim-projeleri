using Microsoft.AspNetCore.Identity;

namespace kuafor_ORMproje.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
    }
}
