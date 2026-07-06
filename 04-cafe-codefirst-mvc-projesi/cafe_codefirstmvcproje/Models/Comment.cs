using cafe_codefirstmvcproje.Models;

public class Comment
{
    public int Id { get; set; }

    public string Text { get; set; }

    public int Rating { get; set; } // 1-5 arası puan

    public bool IsApproved { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }
}