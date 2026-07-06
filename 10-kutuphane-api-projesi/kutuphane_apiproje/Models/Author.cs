namespace kutuphane_apiproje.Models
{
    public class Author
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public ICollection<Book>? Books { get; set; }
    }
}