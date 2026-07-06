namespace kutuphane_apiproje.Models
{
    public class BookCardViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public int AuthorId { get; set; }
        public string? AuthorName { get; set; }

        public string? CategoryName { get; set; }

        public int CategoryId { get; set; }

        public bool IsAvailable { get; set; }

        public DateTime? DueDate { get; set; }
    }
}