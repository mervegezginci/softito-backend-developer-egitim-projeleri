using kurs_javascriptcodefirstproje.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace kurs_javascriptcodefirstproje.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
    }
}
