namespace kutuphane_apiproje.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public int BookId { get; set; }
        public Book? Book { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
    }
}