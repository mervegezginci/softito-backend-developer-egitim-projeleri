using Microsoft.EntityFrameworkCore;

namespace pet_apiproje.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Pet> Pets { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<Toy> Toys { get; set; }

    }
}
