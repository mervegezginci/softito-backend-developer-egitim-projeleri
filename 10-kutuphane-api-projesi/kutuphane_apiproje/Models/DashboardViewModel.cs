namespace kutuphane_apiproje.Models
{
    public class DashboardViewModel
    {
        public int TotalBooks { get; set; }

        public int TotalAuthors { get; set; }

        public int TotalCategories { get; set; }

        public int TotalUsers { get; set; }

        public int ActiveBorrowings { get; set; }

        public int ReturnedBooks { get; set; }

        public int TotalReviews { get; set; }

        public int AvailableBooks { get; set; }

        public int UnavailableBooks { get; set; }
    }
}