namespace kutuphane_apiproje.Models
{
    public class Book
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public int PageCount { get; set; }

        public int PublishYear { get; set; }

        public bool IsAvailable { get; set; } = true;

        public int AuthorId { get; set; }
        public Author? Author { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}