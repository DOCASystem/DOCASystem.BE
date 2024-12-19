using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace DOCA.Domain.Models;

public class DOCADbContext : DbContext
{
    // Default constructor for EF Core to use (for migrations and tools).


    // Constructor that allows setting options (useful for DI).
    public DOCADbContext(DbContextOptions<DOCADbContext> options) : base(options) { }

    // DbSet properties
    public DbSet<User> Users { get; set; }
    public DbSet<Staff> Staffs { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<BlogAnimal> BlogAnimals { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<AnimalImage> AnimalImages { get; set; }
    public DbSet<AnimalCategory> AnimalCategories { get; set; }
    public DbSet<Animal> Animals { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }

    // Configurations for the model using Fluent API
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);

        // Table mapping for each entity
        // modelBuilder.Entity<User>().ToTable("Users");
        // modelBuilder.Entity<Staff>().ToTable("Staffs");
        // modelBuilder.Entity<ProductImage>().ToTable("ProductImages");
        // modelBuilder.Entity<Product>().ToTable("Products");
        // modelBuilder.Entity<Payment>().ToTable("Payments");
        // modelBuilder.Entity<OrderItem>().ToTable("OrderItems");
        // modelBuilder.Entity<Order>().ToTable("Orders");
        // modelBuilder.Entity<Member>().ToTable("Members");
        // modelBuilder.Entity<Category>().ToTable("Categories");
        // modelBuilder.Entity<BlogAnimal>().ToTable("BlogAnimals");
        // modelBuilder.Entity<Blog>().ToTable("Blogs");
        // modelBuilder.Entity<Animal>().ToTable("Animals");
        // modelBuilder.Entity<AnimalImage>().ToTable("AnimalImages");
        // modelBuilder.Entity<AnimalCategory>().ToTable("AnimalCategories");
        // modelBuilder.Entity<ProductCategory>().ToTable("ProductCategories");
    }

    // Configuring the connection string for the database
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // It's recommended to use appsettings.json or environment variables for connection strings
            optionsBuilder.UseSqlServer(
                "Server=MAGIC-FLEXING\\SQLEXPRESS;Database=DOCA;User Id=sa;Password=123456;Encrypt=True;TrustServerCertificate=True"
            );
        }
    }
}
