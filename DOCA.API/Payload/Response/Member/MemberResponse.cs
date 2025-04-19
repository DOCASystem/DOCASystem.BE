using DOCA.API.Payload.Response.Account;
using DOCA.API.Payload.Response.Order;
using DOCA.Domain.Models;

namespace DOCA.API.Payload.Response.Member;

public class MemberResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string Username { get; set; }
    public string PhoneNumber { get; set; }
    public string FullName { get; set; }
    public string? Commune { get; set; }
    public string? District { get; set; }
    public string? Province { get; set; }
    public string? Address { get; set; }
    public string? ProvinceCode { get; set; }
    public string? DistrictCode { get; set; }
    public string? CommuneCode { get; set; }
    public virtual ICollection<OrderResponse>? Orders { get; set; }
}