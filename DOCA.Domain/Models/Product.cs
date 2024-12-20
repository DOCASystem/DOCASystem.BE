using System.ComponentModel.DataAnnotations;

namespace DOCA.Domain.Models;

public class Product
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public bool IsHidden { get; set; }
    
    public virtual ICollection<ProductCategory>? ProductCategories { get; set; }
    
    public virtual ICollection<ProductImage>? ProductImages { get; set; }
}