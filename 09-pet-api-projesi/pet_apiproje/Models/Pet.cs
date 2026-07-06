namespace pet_apiproje.Models
{
    public class Pet
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Species { get; set; } // Kedi, Köpek, Kuş...
        public int Age { get; set; }
    }
}
