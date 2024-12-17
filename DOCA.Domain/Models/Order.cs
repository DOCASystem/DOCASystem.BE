using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOCA.Domain.Models;

public class Order
{
    [Key]
    public Guid id { get; set; }
    public decimal Total { get; set; }
    public int Status { get; set; }
    public string Code { get; set; }
    [MaxLength(1000)]
    public string Address { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public Guid MemberId { get; set; }
    [ForeignKey(nameof(MemberId))]
    public Member Member { get; set; }
    
    public Guid PaymentId { get; set; }
    [ForeignKey(nameof(PaymentId))]
    public Payment Payment { get; set; }
    
    public virtual ICollection<OrderItem> OrderItems { get; set; }
}