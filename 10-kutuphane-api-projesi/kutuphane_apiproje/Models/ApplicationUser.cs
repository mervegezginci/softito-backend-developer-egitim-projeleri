using Microsoft.AspNetCore.Identity;

namespace kutuphane_apiproje.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}