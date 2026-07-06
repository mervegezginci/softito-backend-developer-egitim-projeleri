namespace kutuphane_apiproje.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public ICollection<Book>? Books { get; set; }
    }
}