using System;

namespace seyahat_projesi.Model
{
    public class SystemLog
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Level { get; set; } = "info"; // info, warn, error
        public string? IpAddress { get; set; }
        public string? Details { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
