using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOCA.Domain.Models;

public class ProductImage
{
    [Key]
    public Guid Id { get; set; }
    public string ImageUrl { get; set; }
    
    public bool IsMain { get; set; }
    public Guid ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; }
}