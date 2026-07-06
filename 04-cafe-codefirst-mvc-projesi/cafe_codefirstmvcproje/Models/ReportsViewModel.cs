using System.Collections.Generic;

namespace cafe_codefirstmvcproje.Models
{
    public class ReportsViewModel
    {
        public List<CategoryReportItem> CategoryReports { get; set; } = new();
        public List<ProductReportItem> ProductReports { get; set; } = new();
        public int[] RatingCounts { get; set; } = new int[5];

        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalComments { get; set; }
        public int ApprovedComments { get; set; }
        public int PendingComments { get; set; }
    }

    public class CategoryReportItem
    {
        public string CategoryName { get; set; }
        public int ProductCount { get; set; }
        public double AvgPrice { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
    }

    public class ProductReportItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        public bool IsPopular { get; set; }
        public int CommentCount { get; set; }
        public double AverageRating { get; set; }
    }
}
