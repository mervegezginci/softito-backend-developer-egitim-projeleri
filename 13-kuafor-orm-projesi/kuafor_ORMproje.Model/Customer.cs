namespace kuafor_ORMproje.Model
{
    public class Customer
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Gender { get; set; }

        public DateTime CreatedDate { get; set; }

        public ICollection<Appointment>? Appointments { get; set; }
    }
}
