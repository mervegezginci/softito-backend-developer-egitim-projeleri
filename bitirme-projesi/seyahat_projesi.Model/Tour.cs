using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace seyahat_projesi.Model
{
    public class Tour
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DurationDays { get; set; }
        public double Price { get; set; }
        public int Capacity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public int GuideId { get; set; }
        public Guide? Guide { get; set; }

        [NotMapped]
        public double AverageRating { get; set; } = 4.8;

        [NotMapped]
        public int ReviewCount { get; set; } = 120;
    }
}
