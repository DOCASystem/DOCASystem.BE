using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOCA.Domain.Models;

public class CartItems
{
    [Key]
    public Guid Id { get; set; }
    public Guid CartId { get; set; }
    [ForeignKey(nameof(CartId))]
    public Cart Cart { get; set; }
    public Guid ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; }
    public int Quantity { get; set; }
    public Guid BlogId { get; set; }
    [ForeignKey(nameof(BlogId))]
    public Blog Blog { get; set; }
}