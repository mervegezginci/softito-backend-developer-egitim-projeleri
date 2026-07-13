namespace seyahat_projesi.Model
{
    public class Guide
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string GuideImageUrl { get; set; } = string.Empty;
    }
}
