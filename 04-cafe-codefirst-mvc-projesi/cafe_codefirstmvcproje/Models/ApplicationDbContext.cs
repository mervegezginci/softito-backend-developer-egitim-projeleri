using Microsoft.EntityFrameworkCore;

namespace cafe_codefirstmvcproje.Models
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; } 
        public DbSet<Customer> Customers{ get; set; } 
        public DbSet<Product> Products { get; set; } 
        public DbSet<Comment> Comments{ get; set; }
    }
}
