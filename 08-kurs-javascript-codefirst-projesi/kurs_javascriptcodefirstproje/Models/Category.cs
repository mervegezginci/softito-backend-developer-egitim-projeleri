namespace kurs_javascriptcodefirstproje.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string CategoryName { get; set; }

        public List<Course>? Courses { get; set; }
    }
}
