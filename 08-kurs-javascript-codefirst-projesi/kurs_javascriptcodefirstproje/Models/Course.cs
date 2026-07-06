namespace kurs_javascriptcodefirstproje.Models
{
    public class Course
    {
        public int Id { get; set; }

        public string CourseName { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int Duration { get; set; }

        public string ImageUrl { get; set; }

        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        public List<Enrollment>? Enrollments { get; set; }
    }
}
