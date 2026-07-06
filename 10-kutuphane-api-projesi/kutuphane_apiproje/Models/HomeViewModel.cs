namespace kutuphane_apiproje.Models
{
    public class HomeViewModel
    {
        public List<BookCardViewModel> Books { get; set; } = new();
        public List<Category> Categories { get; set; }

        public List<Author> Authors { get; set; }

        public List<Review> Reviews { get; set; }
        
        public int UserCount { get; set; }

        public List<MostBorrowedBookViewModel> MostBorrowedBooks { get; set; } = new();
    }
}
