using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace emlak_dbfirstmvcproje.Models;

public partial class RealEstateDbContext : DbContext
{
    public RealEstateDbContext()
    {
    }

    public RealEstateDbContext(DbContextOptions<RealEstateDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Property> Properties { get; set; }

    public virtual DbSet<PropertyType> PropertyTypes { get; set; }

    public virtual DbSet<Realtor> Realtors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=RealEstateDb;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cities__3214EC074E5E1C27");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Property>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Properti__3214EC07A26328D2");

            entity.Property(e => e.Address).HasMaxLength(300);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RoomCount).HasMaxLength(20);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.City).WithMany(p => p.Properties)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Properties_Cities");

            entity.HasOne(d => d.PropertyType).WithMany(p => p.Properties)
                .HasForeignKey(d => d.PropertyTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Properties_PropertyTypes");

            entity.HasOne(d => d.Realtor).WithMany(p => p.Properties)
                .HasForeignKey(d => d.RealtorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Properties_Realtors");
        });

        modelBuilder.Entity<PropertyType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Property__3214EC074B235A85");

            entity.Property(e => e.TypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<Realtor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Realtors__3214EC076F69047B");

            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.NameSurname).HasMaxLength(150);
            entity.Property(e => e.Phone).HasMaxLength(30);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
