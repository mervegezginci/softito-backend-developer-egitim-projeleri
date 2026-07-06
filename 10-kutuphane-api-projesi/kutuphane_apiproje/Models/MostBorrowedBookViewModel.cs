namespace kutuphane_apiproje.Models
{
    public class MostBorrowedBookViewModel
    {
        public int BookId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string AuthorName { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public int BorrowCount { get; set; }
    }
}
