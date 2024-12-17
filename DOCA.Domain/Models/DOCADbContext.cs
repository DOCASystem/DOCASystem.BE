using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace DOCA.Domain.Models;

public class DOCADbContext : DbContext
{
    public DOCADbContext()
    {
        
    }

    public DOCADbContext(DbContextOptions<DOCADbContext> options) : base(options)
    {
        
    }
    
    public DbSet<User> User { get; set; }
    public DbSet<Staff> Staff { get; set; }
    public DbSet<ProductImage> ProductImage { get; set; }
    public DbSet<Product> Product { get; set; }
    public DbSet<Payment> Payment { get; set; }
    public DbSet<OrderItem> OrderItem { get; set; }
    public DbSet<Order> Order { get; set; }
    public DbSet<Member> Member { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<BlogAnimal> BlogAnimal { get; set; }
    public DbSet<Blog> Blog { get; set; }
    public DbSet<AnimalImage> AnimalImage { get; set; }
    public DbSet<AnimalCategory> AnimalCategory { get; set; }
    public DbSet<Animal> Animal { get; set; }
    public DbSet<ProductCategory> ProductCategory { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<Staff>().ToTable("Staff");
        modelBuilder.Entity<ProductImage>().ToTable("ProductImage");
        modelBuilder.Entity<Product>().ToTable("Product");
        modelBuilder.Entity<Payment>().ToTable("Payment");
        modelBuilder.Entity<OrderItem>().ToTable("OrderItem");
        modelBuilder.Entity<Order>().ToTable("Order");
        modelBuilder.Entity<Member>().ToTable("Member");
        modelBuilder.Entity<Category>().ToTable("Category");
        modelBuilder.Entity<BlogAnimal>().ToTable("BlogAnimal");
        modelBuilder.Entity<Blog>().ToTable("Blog");
        modelBuilder.Entity<AnimalImage>().ToTable("AnimalImage");
        modelBuilder.Entity<AnimalCategory>().ToTable("AnimalCategory");
        modelBuilder.Entity<Animal>().ToTable("Animal");
        modelBuilder.Entity<ProductCategory>().ToTable("ProductCategory");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // optionsBuilder.UseSqlServer(
            //     "Server=103.238.235.227,1433;Database=KALS-Production;User Id=sa;Password=$Thanhkhoa;Encrypt=True;TrustServerCertificate=True"
            // );
            optionsBuilder.UseSqlServer(
                "Server=MAGIC-FLEXING\\SQLEXPRESS,1433;Database=DOCA;User Id=sa;Password=123456;Encrypt=True;TrustServerCertificate=True"
            );
        }
    }
}