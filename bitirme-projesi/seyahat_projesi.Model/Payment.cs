using System;

namespace seyahat_projesi.Model
{
    public class Payment
    {
        public int Id { get; set; }
        
        public int BookingId { get; set; }
        public Booking? Booking { get; set; }

        public double Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? TransactionId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // pending, completed, failed
    }
}
