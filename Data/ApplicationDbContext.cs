using Bazaarly.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Bazaarly.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Comments> Comments { get; set; }

        public DbSet<Favorites> Favorites { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Addresses> Addresses { get; set; }
        public DbSet<Report> Reports { get; set; }


        //public DbSet<ApplicationUser> ApplicationUsers { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .Property(p => p.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Comments>()
         .HasKey(c => c.CommentId); // Define the primary key
            modelBuilder.Entity<Addresses>()
       .HasKey(a => a.AddressId); // Define the primary key

            modelBuilder.Entity<Addresses>()
                .Property(a => a.UserId)
                .IsRequired(); // Example of making UserId required

            modelBuilder.Entity<Addresses>()
                .Property(a => a.Street)
                .HasMaxLength(100) // Set max length for Street
                .IsRequired(); // Example of making Street required

            modelBuilder.Entity<Addresses>()
                .Property(a => a.City)
                .HasMaxLength(50) // Set max length for City
                .IsRequired(); // Example of making City required

            modelBuilder.Entity<Addresses>()
                .Property(a => a.State)
                .HasMaxLength(50) // Set max length for State
                .IsRequired(); // Example of making State required

            modelBuilder.Entity<Addresses>()
                .Property(a => a.ZipCode)
                .HasMaxLength(10) // Set max length for ZipCode
                .IsRequired(); // Example of making ZipCode required

            modelBuilder.Entity<Addresses>()
                .Property(a => a.Country)
                .HasMaxLength(50) // Set max length for Country
                .IsRequired(); // Example of making Country required
            modelBuilder.Entity<Order>()
        .HasMany(o => o.OrderItems)
        .WithOne(oi => oi.Order)
        .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany() // Assuming Product has no navigation property back to OrderItem
                .HasForeignKey(oi => oi.ProductId);
        }

    }
}