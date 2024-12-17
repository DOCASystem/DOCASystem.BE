using System.ComponentModel.DataAnnotations;

namespace DOCA.Domain.Models;

public class Payment
{
    [Key]
    public Guid Id { get; set; }
    public int OrderCode { get; set; } //OrderId of PayOS
    public DateTime? PaymentDateTime { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public int Status { get; set; }
    public Order Order { get; set; }
}