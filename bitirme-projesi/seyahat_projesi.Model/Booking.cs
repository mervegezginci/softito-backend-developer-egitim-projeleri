using System;

namespace seyahat_projesi.Model
{
    public class Booking
    {
        public int Id { get; set; }
        
        public int TourId { get; set; }
        public Tour? Tour { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public DateTime BookingDate { get; set; }
        public int GuestsCount { get; set; }
        public double TotalPrice { get; set; }
        public string PaymentStatus { get; set; } = "unpaid"; // unpaid, paid, refunded
        public string Status { get; set; } = "pending"; // pending, approved, cancelled

        public int? CouponId { get; set; }
        public Coupon? Coupon { get; set; }

        public Payment? Payment { get; set; }
    }
}
