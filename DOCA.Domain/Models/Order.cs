using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DOCA.API.Enums;

namespace DOCA.Domain.Models;

public class Order
{
    [Key]
    public Guid Id { get; set; }
    
    [Column(TypeName = "decimal(18, 4)")] // Định nghĩa kiểu dữ liệu SQL Server
    public decimal Total { get; set; }
    public OrderStatus  Status { get; set; }
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