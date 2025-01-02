using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOCA.Domain.Models;

public class OrderItem
{
    [Key]
    public Guid Id { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public string WarrantyCode { get; set; }
    public DateTime? WarrantyExpired { get; set; }
    public Guid OrderId { get; set; }
    [ForeignKey(nameof(OrderId))]
    public Order Order { get; set; }
    public Guid BlogID { get; set; }
    [ForeignKey(nameof(BlogID))]
    public Blog Blog { get; set; }
    public Guid ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; }
}