using seyahat_projesi.Model;
using System.Collections.Generic;

namespace seyahat_projesi.ViewModels
{
    public class DashboardViewModel
    {
        public ApplicationUser User { get; set; } = null!;
        public List<Booking> Bookings { get; set; } = null!;
    }
}
