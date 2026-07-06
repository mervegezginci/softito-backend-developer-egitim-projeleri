namespace cafe_codefirstmvcproje.Models
{
    public class HomeViewModel
    {
        public List<Category> Categories { get; set; }

        public List<Product> PopularProducts { get; set; }

        public List<Comment> Comments { get; set; }
    }
}
