using DOCA.API.Enums;
using DOCA.API.Payload.Response.Order;

namespace DOCA.API.Payload.Response.CheckOut;

public class PaymentWithOrderResponse
{
    public Guid Id { get; set; }
    public DateTime PaymentDateTime { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public PaymentStatus Status { get; set; }
    public OrderResponse Order { get; set; }
}