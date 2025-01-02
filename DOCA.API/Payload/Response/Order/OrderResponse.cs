using DOCA.API.Payload.Response.Member;

namespace DOCA.API.Payload.Response.Order;

public class OrderResponse
{
    public Guid Id { get; set; }
    public decimal Total { get; set; }
    public string Address { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    
    public MemberResponse Member { get; set; }
}